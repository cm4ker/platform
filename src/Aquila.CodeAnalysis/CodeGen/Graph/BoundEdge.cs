using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.PooledObjects;
using Aquila.CodeAnalysis.Symbols;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using Aquila.CodeAnalysis.CodeGen;

namespace Aquila.CodeAnalysis.Semantics.Graph
{
    partial class Edge : IGenerator
    {
        /// <summary>
        /// Generates or enqueues next blocks to the worklist.
        /// </summary>
        internal abstract void Generate(CodeGenerator cg);

        void IGenerator.Generate(CodeGenerator cg) => this.Generate(cg);
    }

    partial class SimpleEdge
    {
        internal override void Generate(CodeGenerator cg)
        {
            if (cg.IsDebug && this.AquilaSyntax != null)
            {
                cg.EmitSequencePoint(this.AquilaSyntax);
            }

            cg.Scope.ContinueWith(NextBlock);
        }
    }

    partial class LeaveEdge
    {
        internal override void Generate(CodeGenerator cg)
        {
            // nop
        }
    }

    partial class ConditionalEdge
    {
        internal override void Generate(CodeGenerator cg)
        {
            Contract.ThrowIfNull(Condition);

            // !COND?T:F -> COND?F:T
            var isnegation = this.Condition.IsLogicNegation(out var negexpr);
            var condition = isnegation ? negexpr : this.Condition;

            if (TrueTarget == FalseTarget)
            {
                // condition always results in the same code flow
                cg.EmitSequencePoint(this.Condition.AquilaSyntax);
                cg.EmitPop(cg.Emit(this.Condition));
            }
            else if (IsLoop) // perf
            {
                cg.EmitHiddenSequencePoint();
                cg.Builder.EmitBranch(ILOpCode.Br, condition);

                // {
                cg.GenerateScope(TrueTarget, NextBlock.Ordinal);
                // }

                // if (Condition)
                cg.EmitHiddenSequencePoint();
                cg.Builder.MarkLabel(condition);

                cg.EmitSequencePoint(this.Condition.AquilaSyntax);
                cg.EmitConvert(condition, cg.CoreTypes.Boolean);

                cg.Builder.EmitBranch(isnegation ? ILOpCode.Brfalse : ILOpCode.Brtrue, TrueTarget);
            }
            else
            {
                // if (Condition)
                cg.EmitSequencePoint(this.Condition.AquilaSyntax);
                cg.EmitConvert(condition, cg.CoreTypes.Boolean);

                cg.Builder.EmitBranch(isnegation ? ILOpCode.Brtrue : ILOpCode.Brfalse, FalseTarget);

                // {
                cg.GenerateScope(TrueTarget, NextBlock.Ordinal);
                // }
            }

            cg.EmitHiddenSequencePoint();
            cg.Scope.ContinueWith(FalseTarget);
        }
    }

    partial class TryCatchEdge
    {
        /// <summary>
        /// Whether to emit catch and finally bodies outside the TryCatchFinally scope.
        /// This allows to branch inside catch or finally from outside,
        /// or branch outside of try without calling finally (required for yield and return functionality).
        /// </summary>
        public bool EmitCatchFinallyOutsideScope { get; internal set; }

        internal override void Generate(CodeGenerator cg)
        {
            EmitTryStatement(cg);

            //
            cg.Scope.ContinueWith(NextBlock);
        }

        void EmitTryStatement(CodeGenerator cg, bool emitCatchesOnly = false)
        {
        }

        void EmitScriptDiedBlock(CodeGenerator cg)
        {
        }

        void EmitDefaultCatchBlock(CodeGenerator cg)
        {
        }

        void EmitTypeCheck(CodeGenerator cg, BoundTypeRef tref)
        {
        }

        void EmitMultipleTypeCheck(CodeGenerator cg, ImmutableArray<BoundTypeRef> trefs)
        {
        }

        void EmitCatchBlock(CodeGenerator cg, CatchBlock catchBlock)
        {
        }

        void EmitJumpTable(CodeGenerator cg)
        {
            var yields = cg.Method.ControlFlowGraph.Yields;
            if (yields.IsDefaultOrEmpty)
            {
                return;
            }

            // local <state> = g._state that is switched on (can't switch on remote field)
            Debug.Assert(cg.GeneratorStateLocal != null);

            // create label for situation when state doesn't correspond to continuation: 0 -> didn't run to first yield
            var noContinuationLabel = new NamedLabel("noStateContinuation");

            // prepare jump table from yields
            var yieldExLabels = new List<KeyValuePair<ConstantValue, object>>();
            foreach (var yield in yields)
            {
                // only applies to yields inside this "try" block
                var node = yield.ContainingTryScopes.First;
                while (node != null && node.Value != this)
                {
                    node = node.Next;
                }

                if (node == null) continue;

                // jump to next nested "try" or inside "yield" itself
                var target = (object) node.Next?.Value /*next try block*/ ?? yield /*inside yield*/;

                // case YieldIndex: goto target;
                yieldExLabels.Add(
                    new KeyValuePair<ConstantValue, object>(ConstantValue.Create(yield.YieldIndex), target));
            }

            if (yieldExLabels.Count != 0)
            {
                // emit switch table that based on g._state jumps to appropriate continuation label
                cg.Builder.EmitIntegerSwitchJumpTable(yieldExLabels.ToArray(), noContinuationLabel,
                    cg.GeneratorStateLocal, Microsoft.Cci.PrimitiveTypeCode.Int32);

                cg.Builder.MarkLabel(noContinuationLabel);
            }
        }
    }

    partial class ForeachEnumereeEdge
    {
        internal override void Generate(CodeGenerator cg)
        {
        }
    }

    partial class ForeachMoveNextEdge
    {
        internal override void Generate(CodeGenerator cg)
        {
        }
    }

    partial class MatchEdge
    {
        static bool IsInt32(object value) => value is int ||
                                             (value is long && (long) value <= int.MaxValue &&
                                              (long) value >= int.MinValue);

        static bool IsString(object value) => value is string;

        internal override void Generate(CodeGenerator cg)
        {
        }

        /// <summary>
        /// Gets case labels.
        /// </summary>
        static KeyValuePair<ConstantValue, object>[] GetSwitchCaseLabels(IEnumerable<MatchArmBlock> sections)
        {
            var labelsBuilder = ArrayBuilder<KeyValuePair<ConstantValue, object>>.GetInstance();
            foreach (var section in sections)
            {
                if (section.IsDefault)
                {
                    // fallThroughLabel = section
                }
                else
                {
                    // labelsBuilder.Add(new KeyValuePair<ConstantValue, object>(
                    //     Int32Constant(section.MatchValue.BoundElement.ConstantValue.Value), section));
                }
            }

            return labelsBuilder.ToArrayAndFree();
        }

        // TODO: move to helpers
        static ConstantValue Int32Constant(object value)
        {
            if (value is int) return ConstantValue.Create((int) value);
            if (value is long) return ConstantValue.Create((int) (long) value);
            if (value is double) return ConstantValue.Create((int) (double) value);

            throw new ArgumentException();
        }
    }
}