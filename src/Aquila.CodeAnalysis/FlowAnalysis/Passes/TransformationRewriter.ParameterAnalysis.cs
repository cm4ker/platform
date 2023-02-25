﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Semantics.Graph;
using Aquila.CodeAnalysis.FlowAnalysis;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Source;
using Aquila.CodeAnalysis.Utilities;
using Aquila.Syntax.Ast;

namespace Aquila.CodeAnalysis.FlowAnalysis.Passes
{
    partial class TransformationRewriter
    {
        /// <summary>
        /// The state denotes whether a potential modification of any parameter containing alias could have
        /// happened (e.g. by calling an external method).
        /// </summary>
        private enum ParameterAnalysisState
        {
            /// <summary>
            /// We haven't analysed this code.
            /// </summary>
            Unexplored = default,

            /// <summary>
            /// We have already analysed this code and haven't discovered any operation possibly modifying any alias
            /// outside the method scope.
            /// </summary>
            Clean,

            /// <summary>
            /// Any parameter dereference encountered in this state on prevents its value passing from being skipped, because
            /// a possible alias modification might have happened.
            /// </summary>
            Dirty
        }

        /// <summary>
        /// Implements parameter value passing analysis using <see cref="ParameterAnalysisState"/>, retrieving information
        /// about on which parameters we don't need to call PassValue.
        /// </summary>
        private class ParameterAnalysis : AnalysisWalker<ParameterAnalysisState, VoidStruct>
        {
            #region Fields

            /// <summary>
            /// Records parameters which need to be deep copied and dealiased upon method start.
            /// </summary>
            private BitMask64 _needPassValueParams;

            private FlowContext _flowContext;

            private Dictionary<BoundBlock, ParameterAnalysisState> _blockToStateMap =
                new Dictionary<BoundBlock, ParameterAnalysisState>();

            private DistinctQueue<BoundBlock> _worklist =
                new DistinctQueue<BoundBlock>(new BoundBlock.OrdinalComparer());

            #endregion

            #region Usage

            private ParameterAnalysis(FlowContext flowContext)
            {
                _flowContext = flowContext;
            }

            public static BitMask64 GetNeedPassValueParams(SourceMethodSymbolBase method)
            {
                if (method.ParameterCount == 0)
                {
                    return default;
                }

                var cfg = method.ControlFlowGraph;
                var analysis = new ParameterAnalysis(cfg.FlowContext);

                analysis._blockToStateMap[cfg.Start] = ParameterAnalysisState.Clean;
                analysis._worklist.Enqueue(cfg.Start);
                while (analysis._worklist.TryDequeue(out var block))
                {
                    analysis.Accept(block);
                }

                return analysis._needPassValueParams;
            }

            #endregion

            #region State handling

            protected override bool IsStateInitialized(ParameterAnalysisState state) => state != default;

            protected override bool AreStatesEqual(ParameterAnalysisState a, ParameterAnalysisState b) => a == b;

            protected override ParameterAnalysisState GetState(BoundBlock block) =>
                _blockToStateMap.TryGetOrDefault(block);

            protected override void SetState(BoundBlock block, ParameterAnalysisState state) =>
                _blockToStateMap[block] = state;

            protected override ParameterAnalysisState CloneState(ParameterAnalysisState state) => state;

            protected override ParameterAnalysisState MergeStates(ParameterAnalysisState a, ParameterAnalysisState b) =>
                a > b ? a : b;

            protected virtual void SetStateUnknown(ref ParameterAnalysisState state) =>
                state = ParameterAnalysisState.Dirty;

            protected override void EnqueueBlock(BoundBlock block) => _worklist.Enqueue(block);

            #endregion

            #region Visit expressions

            public override VoidStruct VisitArgument(BoundArgument x)
            {
                VariableHandle varindex;

                if (State != ParameterAnalysisState.Dirty
                    && x.Value is BoundVariableRef varRef
                    && !_flowContext.IsReference(_flowContext.GetVarIndex(varRef.Name.NameValue))
                    && !varRef.Access.MightChange)
                {
                    // Passing a parameter as an argument by value to another method is a safe use which does not
                    // require it to be deeply copied (the called function will do it on its own if necessary)
                    return default;
                }
                else
                {
                    return base.VisitArgument(x);
                }
            }

            public override VoidStruct VisitVariableRef(BoundVariableRef x)
            {
                var varindex = _flowContext.GetVarIndex(x.Name.NameValue);
                if (!_flowContext.IsReference(varindex))
                {
                    // Mark only the specific variable as possibly being changed
                    _needPassValueParams.Set(varindex);
                }
                else
                {
                    // TODO: Mark only those that can be referenced
                    _needPassValueParams.SetAll();
                }


                return base.VisitVariableRef(x);
            }

            private VoidStruct VisitStringConvertingArgs(ImmutableArray<BoundArgument> args)
            {
                // Converting any object argument to string can cause a __toString call
                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    arg.Accept(this);
                }

                return default;
            }

            protected override void Visit(BoundUnaryEx x, ConditionBranch branch)
            {
                base.Visit(x, branch);

                // Cloning causes calling __clone with arbitrary code
                if (x.Operation == Operations.Clone)
                {
                    State = ParameterAnalysisState.Dirty;
                }
            }

            public override VoidStruct VisitConversionEx(BoundConversionEx x)
            {
                base.VisitConversionEx(x);

                return default;
            }

            public override VoidStruct VisitFieldRef(BoundFieldRef x)
            {
                base.VisitFieldRef(x);
                State = ParameterAnalysisState.Dirty;

                return default;
            }

            public override VoidStruct VisitArrayItemEx(BoundArrayItemEx x)
            {
                base.VisitArrayItemEx(x);
                State = ParameterAnalysisState.Dirty;

                return default;
            }

            #endregion
        }
    }
}