using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace Aquila.CodeAnalysis.Semantics.Graph
{
    /// <summary>
    /// Helper class for <see cref="ControlFlowGraph"/> update. Calls Update on each block, edge, statement
    /// and expression in a CFG (but <see cref="VisitCFG(ControlFlowGraph)"/> not supported).
    /// Doesn't contain any infinite recursion protection.
    /// </summary>
    public abstract class GraphUpdater : GraphVisitor<object>
    {
        #region Helper methods

        protected List<T> VisitList<T>(List<T> list) where T : BoundOperation, IAquilaOperation
        {
            if (list == null || list.Count == 0)
            {
                return list;
            }

            List<T> alternate = null;
            for (int i = 0; i < list.Count; i++)
            {
                var orig = list[i];
                var visited = orig?.Accept(this);

                if (visited != orig)
                {
                    if (alternate == null)
                    {
                        alternate = new List<T>(list);
                    }

                    alternate[i] = (T)visited;
                }
            }

            return alternate ?? list;
        }

        protected ImmutableArray<T> VisitImmutableArray<T>(ImmutableArray<T> arr) where T : class, IAquilaOperation
        {
            if (arr.IsDefaultOrEmpty)
            {
                return arr;
            }

            ImmutableArray<T>.Builder alternate = null;
            for (int i = 0; i < arr.Length; i++)
            {
                var orig = arr[i];
                var visited = orig?.Accept(this);

                if (visited != orig)
                {
                    if (alternate == null)
                    {
                        alternate = arr.ToBuilder();
                    }

                    alternate[i] = (T)visited;
                }
            }

            return alternate?.MoveToImmutable() ?? arr;
        }

        protected ImmutableArray<T> VisitBlockImmutableArray<T>(ImmutableArray<T> arr) where T : BoundBlock
        {
            if (arr.IsDefaultOrEmpty)
            {
                return arr;
            }

            ImmutableArray<T>.Builder alternate = null;
            for (int i = 0; i < arr.Length; i++)
            {
                var orig = arr[i];
                var visited = orig?.Accept(this);

                if (visited != orig)
                {
                    if (alternate == null)
                    {
                        alternate = arr.ToBuilder();
                    }

                    alternate[i] = (T)visited;
                }
            }

            return alternate?.MoveToImmutable() ?? arr;
        }

        protected ImmutableArray<KeyValuePair<T1, T2>> VisitImmutableArrayPairs<T1, T2>(
            ImmutableArray<KeyValuePair<T1, T2>> arr)
            where T1 : BoundOperation, IAquilaOperation
            where T2 : BoundOperation, IAquilaOperation
        {
            if (arr.IsDefaultOrEmpty)
            {
                return arr;
            }

            ImmutableArray<KeyValuePair<T1, T2>>.Builder alternate = null;
            for (int i = 0; i < arr.Length; i++)
            {
                var orig = arr[i];
                var visitedKey = orig.Key?.Accept(this);
                var visitedValue = orig.Value?.Accept(this);

                if (visitedKey != orig.Key || visitedValue != orig.Value)
                {
                    if (alternate == null)
                    {
                        alternate = arr.ToBuilder();
                    }

                    alternate[i] = new KeyValuePair<T1, T2>((T1)visitedKey, (T2)visitedValue);
                }
            }

            return alternate?.MoveToImmutable() ?? arr;
        }

        #endregion

        #region ControlFlowGraph

        /// <summary>
        /// Not supported, use <see cref="GraphRewriter"/> instead.
        /// </summary>
        public override object VisitCFG(ControlFlowGraph x)
        {
            throw new NotSupportedException("Use GraphRewriter to correctly update whole CFG.");
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets value indicating the current block is in a conditional scope.
        /// </summary>
        protected bool IsConditional { get; private set; } = false;

        #endregion

        #region Graph.Block

        protected override object DefaultVisitBlock(BoundBlock x) => throw new InvalidOperationException();

        protected virtual Graph.Edge AcceptEdge(BoundBlock from, Graph.Edge edge)
        {
            return (Graph.Edge)edge?.Accept(this);
        }

        public override object VisitCFGBlock(BoundBlock x)
        {
            return x.Update(
                VisitList(x.Statements),
                AcceptEdge(x, x.NextEdge)).WithLocalPropertiesFrom(x);
        }

        public override object VisitCFGStartBlock(StartBlock x)
        {
            return x.Update(
                VisitList(x.Statements),
                AcceptEdge(x, x.NextEdge));
        }

        public override object VisitCFGExitBlock(ExitBlock x)
        {
            Debug.Assert(x.NextEdge == null);

            return x.Update(VisitList(x.Statements));
        }

        public override object VisitCFGCatchBlock(CatchBlock x)
        {
            return x.Update(
                (BoundTypeRef)Accept(x.TypeRef),
                (BoundVariableRef)Accept(x.Variable),
                VisitList(x.Statements),
                AcceptEdge(x, x.NextEdge));
        }

        public override object VisitCFGCaseBlock(MatchArmBlock x)
        {
            return x.Update(
                x.MatchValue, // TODO: Visit also the expressions
                VisitList(x.Statements),
                AcceptEdge(x, x.NextEdge));
        }

        #endregion

        #region Graph.Edge

        protected override object DefaultVisitEdge(Graph.Edge x) => throw new InvalidOperationException();

        public override object VisitCFGSimpleEdge(Graph.SimpleEdge x)
        {
            return x.Update((BoundBlock)Accept(x.Target));
        }

        public override object VisitCFGLeaveEdge(Graph.LeaveEdge x)
        {
            return x.Update((BoundBlock)Accept(x.Target));
        }

        public override object VisitCFGConditionalEdge(Graph.ConditionalEdge x)
        {
            IsConditional = true;

            return x.Update(
                (BoundBlock)Accept(x.TrueTarget),
                (BoundBlock)Accept(x.FalseTarget),
                (BoundExpression)Accept(x.Condition));
        }

        public override object VisitCFGTryCatchEdge(Graph.TryCatchEdge x)
        {
            IsConditional = true;

            return x.Update(
                (BoundBlock)Accept(x.BodyBlock),
                VisitBlockImmutableArray(x.CatchBlocks),
                (BoundBlock)Accept(x.FinallyBlock),
                (BoundBlock)Accept(x.NextBlock));
        }

        public override object VisitCFGForeachEnumereeEdge(Graph.ForeachEnumereeEdge x)
        {
            IsConditional = true;

            var updated = x.Update(
                (BoundBlock)Accept(x.Target),
                (BoundExpression)Accept(x.Enumeree),
                x.AreValuesAliased);

            if (updated != x)
            {
                // Fix reference from the following ForEachMoveNextEdge
                var moveNext = (Graph.ForeachMoveNextEdge)updated.NextBlock.NextEdge;
                Debug.Assert(moveNext.EnumereeEdge == x);
                updated.NextBlock.SetNextEdge(moveNext.Update(
                    moveNext.BodyBlock,
                    moveNext.NextBlock,
                    updated,
                    moveNext.KeyVariable,
                    moveNext.ValueVariable,
                    moveNext.MoveNextSpan));
            }

            return updated;
        }

        public override object VisitCFGForeachMoveNextEdge(Graph.ForeachMoveNextEdge x)
        {
            IsConditional = true;

            return x.Update(
                (BoundBlock)Accept(x.BodyBlock),
                (BoundBlock)Accept(x.NextBlock),
                x.EnumereeEdge, // It updates this reference in its visit instead
                (BoundReferenceEx)Accept(x.KeyVariable),
                (BoundReferenceEx)Accept(x.ValueVariable),
                x.MoveNextSpan);
        }

        public override object VisitCFGSwitchEdge(Graph.MatchEdge x)
        {
            IsConditional = true;

            return x.Update(
                (BoundExpression)Accept(x.SwitchValue),
                VisitBlockImmutableArray(x.MatchBlocks),
                (BoundBlock)Accept(x.NextBlock));
        }

        #endregion

        #region Expressions

        public override object VisitDefault(BoundOperation x)
        {
            return x;
        }

        public override object VisitArgument(BoundArgument x)
        {
            return x.Update(
                (BoundExpression)Accept(x.Value),
                x.ArgumentKind);
        }

        public override object VisitMethodName(BoundMethodName x)
        {
            return x.Update(
                x.NameValue,
                (BoundExpression)Accept(x.NameExpression));
        }

        public override object VisitBinaryEx(BoundBinaryEx x)
        {
            var left = (BoundExpression)Accept(x.Left);
            var right = (BoundExpression)Accept(x.Right);

            return x.Update(
                left
                , right
                , x.Operation
                , x.Left.ResultType);
        }


        public override object VisitMatchArm(BoundMatchArm x)
        {
            var pattern = (BoundExpression)Accept(x.Pattern);
            var when = (BoundExpression)Accept(x.WhenGuard);
            var result = (BoundExpression)Accept(x.MatchResult);

            return x.Update(pattern, when, result, result.ResultType);
        }

        public override object VisitMatchEx(BoundMatchEx x)
        {
            var expression = (BoundExpression)Accept(x.Expression);

            List<BoundMatchArm> updatedArms = new List<BoundMatchArm>();

            BoundMatchArm lastArm = null;

            foreach (var arm in x.Arms)
            {
                var newArm = (BoundMatchArm)Accept(arm);
                updatedArms.Add(newArm);

                lastArm = newArm;
            }

            return x.Update(expression, updatedArms, lastArm.ResultType);
        }


        public override object VisitUnaryEx(BoundUnaryEx x)
        {
            var operand = (BoundExpression)Accept(x.Operand);
            return x.Update(
                operand
                , x.Operation
                , operand.ResultType);
        }

        public override object VisitConversionEx(BoundConversionEx x)
        {
            var operand = (BoundExpression)Accept(x.Operand);
            var type = (BoundTypeRef)Accept(x.TargetType);
            return x.Update(
                operand,
                type,
                type.ResolvedType);
        }

        public override object VisitIncDecEx(BoundIncDecEx x)
        {
            var exp = (BoundReferenceEx)Accept(x.Target);
            return x.Update(
                exp,
                x.IsIncrement,
                x.IsPostfix,
                exp.ResultType
            );
        }

        public override object VisitConditionalEx(BoundConditionalEx x)
        {
            var condition = (BoundExpression)Accept(x.Condition);
            var ifTrue = (BoundExpression)Accept(x.IfTrue);
            var ifFalse = (BoundExpression)Accept(x.IfFalse);
            return x.Update(
                condition,
                ifTrue,
                ifFalse,
                ifTrue.ResultType
            );
        }

        public override object VisitAssignEx(BoundAssignEx x)
        {
            var target = (BoundReferenceEx)Accept(x.Target);
            var value = (BoundExpression)Accept(x.Value);
            return x.Update(
                target,
                value,
                value.ResultType);
        }

        public override object VisitCompoundAssignEx(BoundCompoundAssignEx x)
        {
            var target = (BoundReferenceEx)Accept(x.Target);
            var value = (BoundExpression)Accept(x.Value);
            return x.Update(
                target,
                value,
                x.Operation,
                value.ResultType);
        }

        public override object VisitCallEx(BoundCallEx x)
        {
            var updatedArgs = x.Arguments.Select(a => (BoundArgument)Accept(a)).ToImmutableArray();
            return x.Update(x.MethodSymbol, updatedArgs, x.TypeArguments, x.Instance, x.ResultType);
        }

        public override object VisitVariableName(BoundVariableName x)
        {
            return x.Update(
                x.NameValue,
                (BoundExpression)Accept(x.NameExpression));
        }

        public override object VisitVariableRef(BoundVariableRef x)
        {
            var variable = (BoundVariableName)Accept(x.Name);
            return x.Update(variable, x.ResultType);
        }

        public override object VisitTemporalVariableRef(BoundTemporalVariableRef x)
        {
            Debug.Assert(x.Name.IsDirect);
            return x;
        }

        public override object VisitListEx(BoundListEx x)
        {
            var items = VisitImmutableArrayPairs(x.Items);
            return x.Update(items, x.ResultType);
        }

        public override object VisitFieldRef(BoundFieldRef x)
        {
            return x.Update((BoundExpression)Accept(x.Instance));
        }

        public override object VisitArrayEx(BoundArrayEx x)
        {
            var items = VisitImmutableArrayPairs(x.Items);
            return x.Update(items, x.ResultType);
        }

        public override object VisitArrayItemEx(BoundArrayItemEx x)
        {
            return x.Update(
                (BoundExpression)Accept(x.Array),
                (BoundExpression)Accept(x.Index));
        }

        public override object VisitArrayItemOrdEx(BoundArrayItemOrdEx x)
        {
            return x.Update(
                (BoundExpression)Accept(x.Array),
                (BoundExpression)Accept(x.Index));
        }

        #endregion

        #region Statements

        public override object VisitEmptyStmt(BoundEmptyStmt x)
        {
            return x;
        }

        public override object VisitBlock(Graph.BoundBlock x)
        {
            Debug.Assert(x.NextEdge == null);

            return x.Update(
                VisitList(x.Statements),
                x.NextEdge);
        }

        public override object VisitExpressionStmt(BoundExpressionStmt x)
        {
            return x.Update((BoundExpression)Accept(x.Expression));
        }

        public override object VisitReturnStmt(BoundReturnStmt x)
        {
            return x.Update((BoundExpression)Accept(x.Returned));
        }

        public override object VisitMethodDeclStmt(BoundMethodDeclStmt x)
        {
            return x;
        }

        #endregion
    }
}