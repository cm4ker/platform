using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Symbols;
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

                // <
                cg.GenerateScope(TrueTarget, NextBlock.Ordinal);
                // >

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

                // <
                cg.GenerateScope(TrueTarget, NextBlock.Ordinal);
                // >
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
            // Stack must be empty at beginning of try block.
            cg.Builder.AssertStackEmpty();

            cg.Builder.OpenLocalScope(ScopeType.TryCatchFinally);

            //try
            cg.Builder.OpenLocalScope(ScopeType.Try);
            cg.Scope.ContinueWith(_body);
            cg.Builder.CloseLocalScope();


            //catch
            foreach (var catchBlock in _catchBlocks)
            {
                cg.Builder.OpenLocalScope(ScopeType.Catch, (NamedTypeSymbol)cg.CoreTypes.AqException);
                cg.Builder.AdjustStack(1); // Account for exception on the stack.
                cg.EmitPop(cg.CoreTypes.AqException);
                cg.Scope.ContinueWith(catchBlock);
                cg.Builder.CloseLocalScope();
            }

            //finally
            if (_finallyBlock != null)
            {
                cg.Builder.OpenLocalScope(ScopeType.Finally);
                this._finallyBlock.Emit(cg);
                cg.Builder.CloseLocalScope();
            }

            //close try-catch
            cg.Builder.CloseLocalScope();
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
                var target = (object)node.Next?.Value /*next try block*/ ?? yield /*inside yield*/;

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

    partial class ForeachEdge
    {
        internal override void Generate(CodeGenerator cg)
        {
            // Stack must be empty at beginning of try block.
            cg.Builder.AssertStackEmpty();

            var bi = ForEachStmt.BoundInfo;

            cg.Builder.OpenLocalScope(ScopeType.TryCatchFinally);
            //try
            cg.Builder.OpenLocalScope(ScopeType.Try);
            cg.Scope.ContinueWith(Move);
            cg.Builder.CloseLocalScope();

            //finally
            cg.Builder.OpenLocalScope(ScopeType.Finally);
            bi.DisposeCall.Emit(cg);
            cg.Builder.CloseLocalScope();

            //close try-catch
            cg.Builder.CloseLocalScope();

            End.Emit(cg);
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
        internal override void Generate(CodeGenerator cg)
        {
        }
    }
}