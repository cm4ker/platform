using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.Symbols;
using Roslyn.Utilities;
using System;
using System.Diagnostics;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Semantics.Graph;
using Aquila.CodeAnalysis.Semantics.TypeRef;
using Aquila.Syntax.Ast;


namespace Aquila.CodeAnalysis.FlowAnalysis
{
    /// <summary>
    /// Visits single expressions and project transformations to flow state.
    /// </summary>
    internal class ExpressionAnalysis<T> : AnalysisWalker<FlowState, T>
    {
        #region Fields & Properties

        /// <summary>
        /// The worklist to be used to enqueue next blocks.
        /// </summary>
        internal Worklist<BoundBlock> Worklist { get; }

        /// <summary>
        /// Gets model for symbols resolution.
        /// </summary>
        internal ISymbolProvider Model => _model;

        readonly ISymbolProvider
            _model;

        /// <summary>
        /// Reference to corresponding source method.
        /// </summary>
        protected SourceMethodSymbolBase Method => State.Method;

        protected AquilaCompilation DeclaringCompilation => _model.Compilation;

        #endregion

        #region Helpers

        /// <summary>
        /// In case of a local variable or parameter, gets its name.
        /// </summary>
        VariableName AsVariableName(BoundReferenceEx r)
        {
            if (r is BoundVariableRef vr)
            {
                return vr.Name.NameValue;
            }

            return default;
        }

        /// <summary>
        /// Gets current visibility scope.
        /// </summary>
        protected OverloadsList.VisibilityScope VisibilityScope => new(Method.ContainingType, Method);

        protected void PingSubscribers(ExitBlock exit)
        {
            if (exit != null)
            {
                var wasNotAnalysed = false;

                if (Method != null && !Method.IsReturnAnalysed)
                {
                    Method.IsReturnAnalysed = true;
                    wasNotAnalysed = true;
                }
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Creates an instance of <see cref="ExpressionAnalysis{T}"/> that can analyse a block.
        /// </summary>
        /// <param name="worklist">The worklist to be used to enqueue next blocks.</param>
        /// <param name="model">The semantic context of the compilation.</param>
        public ExpressionAnalysis(Worklist<BoundBlock> worklist, ISymbolProvider model)
        {
            Debug.Assert(model != null);
            _model = model;
            Worklist = worklist;
        }

        #endregion

        #region State and worklist handling

        protected override bool IsStateInitialized(FlowState state) => state != null;

        protected override bool AreStatesEqual(FlowState a, FlowState b) => a.Equals(b);

        protected override FlowState GetState(BoundBlock block) => block.FlowState;

        protected override void SetState(BoundBlock block, FlowState state) => block.FlowState = state;

        protected override FlowState CloneState(FlowState state) => state.Clone();

        protected override FlowState MergeStates(FlowState a, FlowState b) => a.Merge(b);

        protected override void SetStateUnknown(ref FlowState state) => state.SetAllUnknown(true);

        protected override void EnqueueBlock(BoundBlock block) => Worklist.Enqueue(block);

        #endregion

        #region Visit blocks

        public override T VisitCFGExitBlock(ExitBlock x)
        {
            VisitCFGBlock(x);

            return default;
        }

        public override T VisitCFGCatchBlock(CatchBlock x)
        {
            return default;
        }

        #endregion

        #region Declaration Statements

        public override T VisitStaticVarStmt(BoundStaticVarStmt x)
        {
            var v = x.Declaration;
            var local = State.GetLocalHandle(new VariableName(v.Name));

            State.SetVarKind(local, VariableKind.StaticVariable);

            var oldtype = State.GetLocalType(local);

            // set var
            if (v.InitialValue != null)
            {
                // analyse initializer
                Accept(v.InitialValue);

                bool isInt = v.InitialValue.ConstantValue.IsInteger(out long intVal);
                State.SetLessThanLongMax(local, isInt && intVal < long.MaxValue);
                State.SetGreaterThanLongMin(local, isInt && intVal > long.MinValue);

                State.SetLocalType(local, oldtype);
            }
            else
            {
                State.SetLessThanLongMax(local, false);
                State.SetGreaterThanLongMin(local, false);
                State.SetLocalType(local, oldtype);
                // TODO: explicitly State.SetLocalUninitialized() ?
            }

            return default;
        }

        // public override T VisitGlobalStatement(BoundGlobalVariableStatement x)
        // {
        //     base.VisitGlobalStatement(x); // Accept(x.Variable)
        //
        //     return default;
        // }

        #endregion

        #region Visit Literals

        public override T VisitLiteral(BoundLiteral x)
        {
            return default;
        }

        #endregion

        #region Visit Assignments

        public override T VisitAssignEx(BoundAssignEx x)
        {
            Debug.Assert(x.Target.Access.IsWrite);
            Debug.Assert(x.Value.Access.IsRead);

            //
            Accept(x.Value);

            // new target access with resolved target type
            Visit(x.Target, BoundAccess.None);

            return default;
        }

        public override T VisitGroupedEx(BoundGroupedEx arg)
        {
            arg.Expressions.ForEach(x => Visit(x, x.Access));

            return default;
        }

        public override T VisitCompoundAssignEx(BoundCompoundAssignEx x)
        {
            Debug.Assert(x.Target.Access.IsRead && x.Target.Access.IsWrite);
            Debug.Assert(x.Value.Access.IsRead);

            // Target X Value
            var tmp = new BoundBinaryEx(x.Target.WithAccess(BoundAccess.Read), x.Value,
                AstUtils.CompoundOpToBinaryOp(x.Operation), x.Target.ResultType);
            Visit(tmp, ConditionBranch.AnyResult);

            // Target =
            Visit(x.Target, BoundAccess.Write);

            // put read access back
            x.Target.Access = x.Target.Access.WithRead();

            return default;
        }

        protected virtual void VisitSuperglobalVariableRef(BoundVariableRef x)
        {
        }

        protected virtual void VisitLocalVariableRef(BoundVariableRef x, VariableHandle local)
        {
            Debug.Assert(local.IsValid);

            if (Method == null)
            {
                // invalid use of variable:
                return;
            }

            var previoustype = State.GetLocalType(local); // type of the variable in the previous state


            // bind variable place
            if (x.Variable == null)
            {
                if (Method.LocalsTable.TryGetVariable(local.Name, out var localVar))
                    x.Variable = localVar;
            }

            //
            State.VisitLocal(local);
        }

        public override T VisitVariableRef(BoundVariableRef x)
        {
            if (x.Name.IsDirect)
            {
                // direct variable access:
                if (x.Name.NameValue.IsAutoGlobal)
                {
                    VisitSuperglobalVariableRef(x);
                }
                else
                {
                    VisitLocalVariableRef(x, State.GetLocalHandle(x.Name.NameValue));
                }
            }
            else
            {
            }

            return default;
        }

        public override T VisitPropertyRef(BoundPropertyRef x)
        {
            Visit(x.Instance, BoundAccess.Invoke);
            return base.VisitPropertyRef(x);
        }

        public override T VisitIncDecEx(BoundIncDecEx x)
        {
            Visit(x.Target, BoundAccess.ReadAndWrite);
            return default;
        }

        #endregion

        #region Visit BinaryEx

        private void VisitShortCircuitOp(BoundExpression lExpr, BoundExpression rExpr, bool isAndOp,
            ConditionBranch branch)
        {
            // Each operand has to be evaluated in various states and then the state merged.
            // Simulates short-circuit evaluation in runtime:

            var state = this.State; // original state

            if (branch == ConditionBranch.AnyResult)
            {
                if (isAndOp)
                {
                    // A == True && B == Any
                    // A == False

                    State = state.Clone();
                    VisitCondition(lExpr, ConditionBranch.ToTrue);
                    VisitCondition(rExpr, ConditionBranch.AnyResult);
                    var tmp = State;
                    State = state.Clone();
                    VisitCondition(lExpr, ConditionBranch.ToFalse);
                    State = State.Merge(tmp);
                }
                else
                {
                    // A == False && B == Any
                    // A == True

                    State = state.Clone();
                    VisitCondition(lExpr, ConditionBranch.ToFalse);
                    VisitCondition(rExpr, ConditionBranch.AnyResult);
                    var tmp = State;
                    State = state.Clone();
                    VisitCondition(lExpr, ConditionBranch.ToTrue);
                    State = State.Merge(tmp);
                }
            }
            else if (branch == ConditionBranch.ToTrue)
            {
                if (isAndOp)
                {
                    // A == True && B == True

                    VisitCondition(lExpr, ConditionBranch.ToTrue);
                    VisitCondition(rExpr, ConditionBranch.ToTrue);
                }
                else
                {
                    // A == False && B == True
                    // A == True

                    State = state.Clone();
                    VisitCondition(lExpr, ConditionBranch.ToFalse);
                    VisitCondition(rExpr, ConditionBranch.ToTrue);
                    var tmp = State;
                    State = state.Clone();
                    VisitCondition(lExpr, ConditionBranch.ToTrue);
                    State = State.Merge(tmp);
                }
            }
            else if (branch == ConditionBranch.ToFalse)
            {
                if (isAndOp)
                {
                    // A == True && B == False
                    // A == False

                    State = state.Clone();
                    VisitCondition(lExpr, ConditionBranch.ToTrue);
                    VisitCondition(rExpr, ConditionBranch.ToFalse);
                    var tmp = State;
                    State = state.Clone();
                    VisitCondition(lExpr, ConditionBranch.ToFalse);
                    State = State.Merge(tmp);
                }
                else
                {
                    // A == False && B == False

                    VisitCondition(lExpr, ConditionBranch.ToFalse);
                    VisitCondition(rExpr, ConditionBranch.ToFalse);
                }
            }
        }

        Optional<object> ResolveBooleanOperation(Optional<object> xobj, Optional<object> yobj, Operations op)
        {
            if (xobj.TryConvertToBool(out var bx) && yobj.TryConvertToBool(out var by))
            {
                switch (op)
                {
                    case Operations.And: return (bx && by).AsOptional();
                    case Operations.Or: return (bx || by).AsOptional();
                    case Operations.Xor: return (bx ^ by).AsOptional();
                    default:
                        throw ExceptionUtilities.Unreachable;
                }
            }

            return default;
        }

        /// <summary>
        /// Resolves value of bit operation.
        /// </summary>
        /// <remarks>TODO: move to **evaluation**.</remarks>
        Optional<object> ResolveBitOperation(Optional<object> xobj, Optional<object> yobj, Operations op)
        {
            var xconst = xobj.ToConstantValueOrNull();
            var yconst = yobj.ToConstantValueOrNull();

            if (xconst.TryConvertToLong(out long xval) && yconst.TryConvertToLong(out long yval))
            {
                long result;

                switch (op)
                {
                    case Operations.BitOr:
                        result = xval | yval;
                        break;
                    case Operations.BitAnd:
                        result = xval & yval;
                        break;
                    case Operations.BitXor:
                        result = xval ^ yval;
                        break;
                    default:
                        throw new ArgumentException(nameof(op));
                }

                //
                if (result >= int.MinValue && result <= int.MaxValue)
                {
                    return (int)result;
                }
                else
                {
                    return result;
                }

                //
            }

            return default(Optional<object>);
        }

        protected override void Visit(BoundBinaryEx x, ConditionBranch branch)
        {
            Visit(x.Left, BoundAccess.Read);
            Visit(x.Right, BoundAccess.Read);
        }


        /// <summary>
        /// If possible, resolve the comparison operation in compile-time.
        /// </summary>
        static Optional<object> ResolveComparison(Operations op, object lvalue, object rvalue)
        {
            // TODO

            //
            return default(Optional<object>);
        }

        static Optional<object> ResolveShift(Operations op, Optional<object> lvalue, Optional<object> rvalue)
        {
            if (lvalue.TryConvertToLong(out var left) && rvalue.TryConvertToLong(out var right))
            {
                switch (op)
                {
                    case Operations.ShiftLeft:
                        return (left << (int)right).AsOptional();

                    case Operations.ShiftRight:
                        return (left >> (int)right).AsOptional();

                    default:
                        Debug.Fail("unexpected");
                        break;
                }
            }

            return default;
        }

        /// <summary>
        /// Resolves variable types and potentially assigns a constant boolean value to an expression of a comparison of
        /// a variable and a constant - operators ==, !=, === and !==. Returns true iff this expression was handled and there
        /// is no need to analyse it any more (adding constant value etc.).
        /// </summary>
        private bool ResolveEqualityWithConstantValue(
            BoundBinaryEx cmpExpr,
            BoundReferenceEx refExpr,
            Optional<object> value,
            ConditionBranch branch)
        {
            Debug.Assert(branch != ConditionBranch.AnyResult);

            if (value.IsNull() && refExpr is BoundVariableRef varRef)
            {
                bool isStrict = (cmpExpr.Operation == Operations.Identical ||
                                 cmpExpr.Operation == Operations.NotIdentical);

                bool isPositive = (cmpExpr.Operation == Operations.Equal || cmpExpr.Operation == Operations.Identical);

                // We cannot say much about the type of $x in the true branch of ($x == null) and the false branch of ($x != null),
                // because it holds for false, 0, "", array() etc.
                if (isStrict || branch.TargetValue() != isPositive)
                {
                    AnalysisFacts.HandleTypeCheckingExpression(
                        varRef,
                        branch,
                        State,
                        checkExpr: cmpExpr,
                        isPositiveCheck: isPositive);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to infer the result of an equality comparison from the types of the operands.
        /// </summary>
        private void ResolveEquality(BoundBinaryEx cmpExpr)
        {
            Debug.Assert(cmpExpr.Operation >= Operations.Equal && cmpExpr.Operation <= Operations.NotIdentical);

            bool isStrict = (cmpExpr.Operation == Operations.Identical || cmpExpr.Operation == Operations.NotIdentical);

            if (isStrict && !cmpExpr.Left.CanHaveSideEffects() && !cmpExpr.Right.CanHaveSideEffects())
            {
                // // Always returns false if checked for strict equality and the operands are of different types (and vice versa for strict non-eq)
                // bool isPositive = (cmpExpr.Operation == Operations.Equal || cmpExpr.Operation == Operations.Identical);
                // bool canBeSameType =
                //     Method.TypeRefContext.CanBeSameType(cmpExpr.Left.TypeRefMask, cmpExpr.Right.TypeRefMask);
                // cmpExpr.ConstantValue = !canBeSameType ? (!isPositive).AsOptional() : default;
            }
        }

        #endregion

        #region Visit UnaryEx

        ConstantValue ResolveUnaryMinus(ConstantValue value)
        {
            if (value != null)
            {
                switch (value.SpecialType)
                {
                    case SpecialType.System_Double:
                        return ConstantValue.Create(-value.DoubleValue);

                    case SpecialType.System_Int32:
                        return value.Int32Value != int.MinValue
                            ? ConstantValue.Create(-value.Int32Value) // (- Int32.MinValue) overflows to int64
                            : ConstantValue.Create(-(long)value.Int32Value);

                    case SpecialType.System_Int64:
                        return (value.Int64Value != long.MinValue) // (- Int64.MinValue) overflows to double
                            ? ConstantValue.Create(-value.Int64Value)
                            : ConstantValue.Create(-(double)value.Int64Value);
                    default:
                        break;
                }
            }

            return null;
        }

        #endregion

        #region Visit Conversion

        public override T VisitConversionEx(BoundConversionEx x)
        {
            base.VisitConversionEx(x);

            // evaluate if possible

            if (x.TargetType is BoundPrimitiveTypeRef pt)
            {
                switch (pt.TypeCode)
                {
                    case AquilaTypeCode.Boolean:
                        if (x.Operand.ConstantValue.TryConvertToBool(out bool constBool))
                        {
                            x.ConstantValue = ConstantValueExtensions.AsOptional(constBool);
                        }

                        break;

                    case AquilaTypeCode.Long:
                        if (x.Operand.ConstantValue.TryConvertToLong(out long l))
                        {
                            x.ConstantValue = new Optional<object>(l);
                        }

                        break;

                    case AquilaTypeCode.Double:
                        break;

                    case AquilaTypeCode.String:
                    case AquilaTypeCode.WritableString:
                        if (x.Operand.ConstantValue.TryConvertToString(out string str))
                        {
                            x.ConstantValue = new Optional<object>(str);
                        }

                        break;

                    // case AquilaTypeCode.Object:
                    //     if (IsClassOnly(x.Operand.TypeRefMask))
                    //     {
                    //         // it is object already, keep its specific type
                    //         x.TypeRefMask = x.Operand.TypeRefMask; // (object)<object>
                    //         return default;
                    //     }
                    //
                    //     break;
                }
            }

            //

            // x.TypeRefMask = x.TargetType.GetTypeRefMask(TypeCtx);

            return default;
        }

        #endregion

        #region TypeRef

        public override T VisitTypeRef(BoundTypeRef tref)
        {
            Debug.Assert(!(tref is BoundMultipleTypeRef));

            // resolve type symbol
            tref.ResolvedType = (TypeSymbol)tref.ResolveTypeSymbol(DeclaringCompilation);

            return default;
        }

        #endregion

        #region Visit Function Call

        public override T VisitCallEx(BoundCallEx arg)
        {
            if (arg.Instance != null)
                Visit(arg.Instance, BoundAccess.Invoke);

            arg.Arguments.ForEach(x => VisitArgument(x));

            return base.VisitCallEx(arg);
        }

        /// <summary>
        /// Bind arguments to target method and resolve resulting <see cref="BoundExpression.TypeRefMask"/>.
        /// Expecting <see cref="BoundCallEx.TargetMethod"/> is resolved.
        /// If the target method cannot be bound at compile time, <see cref="BoundCallEx.TargetMethod"/> is nulled.
        /// </summary>
        void BindMethodCall(BoundCallEx x, bool maybeOverload = false)
        {
            if (MethodSymbolExtensions.IsValidMethod(x.MethodSymbol))
            {
                //x.TypeRefMask = BindValidMethodCall(x, x.MethodSymbol, x.ArgumentsInSourceOrder, maybeOverload);
            }
            else if (x.TargetMethod is MissingMethodSymbol || x.TargetMethod == null)
            {
                // we don't know anything about the target callsite,
                // locals passed as arguments should be marked as possible refs:
                foreach (var arg in x.ArgumentsInSourceOrder)
                {
                    if (arg.Value is BoundVariableRef bvar && bvar.Name.IsDirect && !arg.IsUnpacking)
                    {
                        State.SetLocalRef(State.GetLocalHandle(bvar.Name.NameValue));
                    }
                }
            }
            else if (x.TargetMethod is AmbiguousMethodSymbol ambiguity)
            {
                // get the return type from all the ambiguities:
                if (!maybeOverload && x.Access.IsRead)
                {
                }
            }

            if (x.Access.IsReadRef)
            {
            }
        }

        #endregion

        #region Visit FieldRef

        public override T VisitFieldRef(BoundFieldRef x)
        {
            Accept(x.Instance);

            return default;
        }

        #endregion

        #region Visit ArrayEx, ArrayItemEx, ArrayItemOrdEx

        public override T VisitArrayEx(BoundArrayEx x)
        {
            return default;
        }

        public override T VisitArrayItemEx(BoundArrayItemEx x)
        {
            Accept(x.Array);
            Accept(x.Index);

            return default;
        }

        public override T VisitArrayItemOrdEx(BoundArrayItemOrdEx x)
        {
            Accept(x.Array);
            Accept(x.Index);

            // ord($s[$i]) cannot be used as an l-value
            Debug.Assert(!x.Access.MightChange);

            // x.TypeRefMask = TypeCtx.GetLongTypeMask();

            return base.VisitArrayItemOrdEx(x);
        }

        #endregion

        #region VisitLambda

        #endregion

        #region VisitYield

        public override T VisitYieldStmt(BoundYieldStmt x)
        {
            base.VisitYieldStmt(x);

            return default;
        }

        #endregion

        #region VisitMatch

        public override T VisitMatchEx(BoundMatchEx x)
        {
            Accept(x.Expression);

            foreach (var arm in x.Arms)
            {
                Accept(arm);
            }

            return default;
        }


        public override T VisitMatchArm(BoundMatchArm x)
        {
            Accept(x.Pattern);
            Accept(x.MatchResult);
            Accept(x.WhenGuard);

            return default;
        }

        #endregion

        #region Visit

        public override T VisitListEx(BoundListEx x)
        {
            return default;
        }

        public override T VisitConditionalEx(BoundConditionalEx x)
        {
            return default;
        }

        public override T VisitReturnStmt(BoundReturnStmt x)
        {
            if (x.Returned != null)
            {
                Accept(x.Returned);
            }
            else
            {
            }

            return default;
        }

     
        #endregion
    }
}