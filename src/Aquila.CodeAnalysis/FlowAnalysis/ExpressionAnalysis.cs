using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.Symbols;
using Roslyn.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Semantics.Graph;
using Aquila.CodeAnalysis.Semantics.TypeRef;
using Aquila.Compiler.Utilities;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Syntax;
using TextSpan = Microsoft.CodeAnalysis.Text.TextSpan;


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
        protected SourceMethodSymbol Method => State.Method;

        /// <summary>
        /// Gets current type context for type masks resolving.
        /// </summary>
        internal TypeRefContext TypeCtx => State.TypeRefContext;

        protected AquilaCompilation DeclaringCompilation => _model.Compilation;

        protected BoundTypeRefFactory BoundTypeRefFactory => DeclaringCompilation.TypeRefFactory;

        #endregion

        #region Helpers

        /// <summary>
        /// In case given expression is a local or parameter reference,
        /// gets its variable handle within <see cref="State"/>.
        /// </summary>
        VariableHandle TryGetVariableHandle(BoundExpression expr)
        {
            var varname = AsVariableName(expr as BoundReferenceEx);
            if (varname.IsValid())
            {
                return State.GetLocalHandle(varname);
            }
            else
            {
                return default(VariableHandle);
            }
        }

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

        bool IsLongConstant(BoundExpression expr, long value)
        {
            if (expr.ConstantValue.HasValue)
            {
                if (expr.ConstantValue.Value is long) return ((long)expr.ConstantValue.Value) == value;
                if (expr.ConstantValue.Value is int) return ((int)expr.ConstantValue.Value) == value;
            }

            return false;
        }

        bool BindConstantValue(BoundExpression target, FieldSymbol symbol)
        {
            // if (symbol != null && symbol.IsConst)
            // {
            //     var cvalue = symbol.GetConstantValue(false);
            //     target.ConstantValue = (cvalue != null) ? new Optional<object>(cvalue.Value) : null;
            //
            //     if (cvalue != null && cvalue.IsNull)
            //     {
            //         target.TypeRefMask = TypeCtx.GetNullTypeMask();
            //         return true;
            //     }
            //
            //     target.TypeRefMask = TypeRefFactory.CreateMask(TypeCtx, symbol.Type, notNull: true);
            //
            //     return true;
            // }

            return false;
        }

        /// <summary>
        /// Finds the root of given chain, i.e.:
        /// $a : $a
        /// $$a : $a
        /// $a->b : $a
        /// $a[..] : $a
        /// $a->foo() : $a
        /// etc.
        /// </summary>
        /// <remarks>If given expression 'isset', its root returned by this method must be set as well.</remarks>
        internal BoundExpression TryGetExpressionChainRoot(BoundExpression x)
        {
            if (x != null)
            {
                if (x is BoundVariableRef v)
                    return v.Name.IsDirect ? v : TryGetExpressionChainRoot(v.Name.NameExpression);
                if (x is BoundFieldRef f)
                    return TryGetExpressionChainRoot(f.Instance ??
                                                     (f.ContainingType as BoundIndirectTypeRef)?.TypeExpression);
                // if (x is BoundInstanceFunctionCall m) return TryGetExpressionChainRoot(m.Instance);
                if (x is BoundArrayItemEx a) return TryGetExpressionChainRoot(a.Array);
            }

            return null;
        }

        /// <summary>
        /// Gets current visibility scope.
        /// </summary>
        protected OverloadsList.VisibilityScope VisibilityScope =>
            new OverloadsList.VisibilityScope((NamedTypeSymbol)TypeCtx.SelfType, Method);

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

                // Ping the subscribers either if the return type has changed or
                // it is the first time the analysis reached the method exit
                // var rtype = State.GetReturnType();
                // if (rtype != exit._lastReturnTypeMask || wasNotAnalysed)
                // {
                //     exit._lastReturnTypeMask = rtype;
                //     var subscribers = exit.Subscribers;
                //     if (subscribers.Count != 0)
                //     {
                //         lock (subscribers)
                //         {
                //             foreach (var subscriber in subscribers)
                //             {
                //                 Worklist.PingReturnUpdate(exit, subscriber);
                //             }
                //         }
                //     }
                // }
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

            // TODO: EdgeToCallers:
            PingSubscribers(x);

            return default;
        }

        public override T VisitCFGCatchBlock(CatchBlock x)
        {
            // VisitCFGBlockInit(x);
            //
            // // add catch control variable to the state
            // x.TypeRef.Accept(this);
            //
            // if (x.Variable != null)
            // {
            //     x.Variable.Access = BoundAccess.Write.WithWrite(x.TypeRef.GetTypeRefMask(TypeCtx));
            //     State.SetLocalType(State.GetLocalHandle(x.Variable.Name.NameValue), x.Variable.Access.WriteMask);
            //     Accept(x.Variable);
            //
            //     //
            //     x.Variable.ResultType = (TypeSymbol) x.TypeRef.Type;
            // }
            //
            //
            // //
            // DefaultVisitBlock(x);
            //
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

        #region Visit CopyValue

        // public override T VisitCopyValue(BoundCopyValue x)
        // {
        //     Accept(x.Expression);
        //
        //     var tmask = x.Expression.TypeRefMask;
        //
        //     if (tmask.IsRef)
        //     {
        //         // copied value is possible a reference,
        //         // might be anything:
        //         tmask = TypeRefMask.AnyType;
        //     }
        //
        //     // the result is not a reference for sure:
        //     Debug.Assert(!tmask.IsRef);
        //
        //     x.TypeRefMask = tmask;
        //
        //     return default;
        // }

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

        public override T VisitCompoundAssignEx(BoundCompoundAssignEx x)
        {
            Debug.Assert(x.Target.Access.IsRead && x.Target.Access.IsWrite);
            Debug.Assert(x.Value.Access.IsRead);

            // Target X Value
            var tmp = new BoundBinaryEx(x.Target.WithAccess(BoundAccess.Read), x.Value,
                AstUtils.CompoundOpToBinaryOp(x.Operation));
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
                var span =
                    x.Variable = Method.LocalsTable.BindLocalVariable(local.Name,
                        x.AquilaSyntax?.Span.ToTextSpan() ?? TextSpan.FromBounds(0, 0));
            }

            //
            State.VisitLocal(local);

            // update state
            if (x.Access.IsRead)
            {
            }

            if (x.Access.IsWrite)
            {
            }

            if (x.Access.IsUnset)
            {
            }
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

        protected override void Visit(BoundUnaryEx x, ConditionBranch branch)
        {
            // x.TypeRefMask = ResolveUnaryOperatorExpression(x, branch);
        }

        // TypeRefMask ResolveUnaryOperatorExpression(BoundUnaryEx x, ConditionBranch branch)
        // {
        //     if (branch != ConditionBranch.AnyResult && x.Operation == Operations.LogicNegation)
        //     {
        //         // Negation swaps the branches
        //         VisitCondition(x.Operand, branch.NegativeBranch());
        //     }
        //     else
        //     {
        //         Accept(x.Operand);
        //     }
        //
        //     // clear any previous resolved constant 
        //     x.ConstantValue = default(Optional<object>);
        //
        //     //
        //     switch (x.Operation)
        //     {
        //         case Operations.AtSign:
        //             return x.Operand.TypeRefMask;
        //
        //         case Operations.BitNegation:
        //             if (x.Operand.ConstantValue.HasValue)
        //             {
        //                 if (x.Operand.ConstantValue.Value is long l)
        //                 {
        //                     x.ConstantValue = new Optional<object>(~l);
        //                 }
        //                 else if (x.Operand.ConstantValue.Value is int i)
        //                 {
        //                     x.ConstantValue = new Optional<object>(~(long) i);
        //                 }
        //             }
        //
        //             return TypeCtx.GetLongTypeMask(); // TODO: or byte[]
        //
        //         case Operations.Clone:
        //             // result is always object, not aliased
        //             return TypeCtx.GetObjectsFromMask(x.Operand.TypeRefMask).IsVoid
        //                 ? TypeCtx.GetSystemObjectTypeMask() // "object"
        //                 : TypeCtx.GetObjectsFromMask(x.Operand.TypeRefMask); // (object)T
        //
        //         case Operations.LogicNegation:
        //         {
        //             if (x.Operand.ConstantValue.TryConvertToBool(out bool constBool))
        //             {
        //                 x.ConstantValue = ConstantValueExtensions.AsOptional(!constBool);
        //             }
        //
        //             return TypeCtx.GetBooleanTypeMask();
        //         }
        //
        //         case Operations.Minus:
        //             var cvalue = ResolveUnaryMinus(x.Operand.ConstantValue.ToConstantValueOrNull());
        //             if (cvalue != null)
        //             {
        //                 x.ConstantValue = new Optional<object>(cvalue.Value);
        //                 return TypeCtx.GetTypeMask(BoundTypeRefFactory.Create(cvalue), false);
        //             }
        //             else
        //             {
        //                 if (IsDoubleOnly(x.Operand))
        //                 {
        //                     return TypeCtx.GetDoubleTypeMask(); // double in case operand is double
        //                 }
        //
        //                 return TypeCtx.GetNumberTypeMask(); // TODO: long in case operand is not a number
        //             }
        //
        //         case Operations.UnsetCast:
        //             return TypeCtx.GetNullTypeMask(); // null
        //
        //         case Operations.Plus:
        //             if (IsNumberOnly(x.Operand.TypeRefMask))
        //                 return x.Operand.TypeRefMask;
        //             return TypeCtx.GetNumberTypeMask();
        //
        //         case Operations.Print:
        //             return TypeCtx.GetLongTypeMask();
        //
        //         default:
        //             throw ExceptionUtilities.Unreachable;
        //     }
        // }

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

        #region Visit InstanceOf

        // protected override void Visit(BoundInstanceOfEx x, ConditionBranch branch)
        // {
        //     Accept(x.Operand);
        //     x.AsType.Accept(this);
        //
        //     // TOOD: x.ConstantValue // in case we know and the operand is a local variable (we can ignore the expression and emit result immediatelly)
        //
        //     var opTypeMask = x.Operand.TypeRefMask;
        //     if (x.Operand is BoundLiteral
        //         || (!opTypeMask.IsAnyType && !opTypeMask.IsRef && !Method.TypeRefContext.IsObject(opTypeMask)))
        //     {
        //         x.ConstantValue = ConstantValueExtensions.AsOptional(false);
        //     }
        //     else if (x.Operand is BoundVariableRef vref && vref.Name.IsDirect)
        //     {
        //         if (branch == ConditionBranch.ToTrue)
        //         {
        //             // if (Variable is T) => variable is T in True branch state
        //             var vartype = x.AsType.GetTypeRefMask(TypeCtx);
        //             if (opTypeMask.IsRef) vartype = vartype.WithRefFlag; // keep IsRef flag
        //
        //             State.SetLocalType(State.GetLocalHandle(vref.Name.NameValue), vartype);
        //         }
        //     }
        //
        //     //
        //     x.TypeRefMask = TypeCtx.GetBooleanTypeMask();
        // }

        #endregion

        #region Visit IsSet, OffsetExists

        //
        // protected override void Visit(BoundIsSetEx x, ConditionBranch branch)
        // {
        //     Accept(x.VarReference);
        //
        //     // try to get resulting value and type of the variable
        //     var localname = AsVariableName(x.VarReference);
        //     if (localname.IsValid())
        //     {
        //         var handle = State.GetLocalHandle(localname);
        //         Debug.Assert(handle.IsValid);
        //
        //         // Remove any constant value of isset()
        //         x.ConstantValue = default;
        //
        //         //
        //         if (State.IsLocalSet(handle))
        //         {
        //             // If the variable is always defined, isset() behaves like !is_null()
        //             var currenttype = State.GetLocalType(handle);
        //
        //             // a type in the true branch:
        //             var positivetype = TypeCtx.WithoutNull(currenttype);
        //
        //             // resolve the constant if possible,
        //             // does not depend on the branch
        //             if (!currenttype.IsRef && !currenttype.IsAnyType)
        //             {
        //                 if (positivetype.IsVoid) // always false
        //                 {
        //                     x.ConstantValue = ConstantValueExtensions.AsOptional(false);
        //                 }
        //                 else if (positivetype == currenttype) // not void nor null
        //                 {
        //                     x.ConstantValue = ConstantValueExtensions.AsOptional(true);
        //                 }
        //             }
        //
        //             // we can be more specific in true/false branches:
        //             if (branch != ConditionBranch.AnyResult && !x.ConstantValue.HasValue)
        //             {
        //                 // update target type in true/false branch:
        //                 var newtype = (branch == ConditionBranch.ToTrue)
        //                     ? positivetype
        //                     : TypeCtx.GetNullTypeMask();
        //
        //                 // keep the flags
        //                 newtype |= currenttype.Flags;
        //
        //                 //
        //                 State.SetLocalType(handle, newtype);
        //             }
        //         }
        //         else if (localname.IsAutoGlobal)
        //         {
        //             // nothing
        //         }
        //         else
        //         {
        //             // variable is not set for sure
        //             // isset : false
        //             x.ConstantValue = ConstantValueExtensions.AsOptional(false);
        //         }
        //
        //         // mark variable as either initialized or uninintialized in respective branches
        //         if (branch == ConditionBranch.ToTrue)
        //         {
        //             State.SetVarInitialized(handle);
        //         }
        //     }
        //
        //     // always returns a boolean
        //     x.TypeRefMask = TypeCtx.GetBooleanTypeMask();
        // }
        //
        // public override T VisitOffsetExists(BoundOffsetExists x)
        // {
        //     // receiver[index]
        //     base.VisitOffsetExists(x);
        //
        //     // TODO: if receiver is undefined -> result is false
        //
        //     // always bool
        //     x.ResultType = DeclaringCompilation.CoreTypes.Boolean;
        //     x.TypeRefMask = TypeCtx.GetBooleanTypeMask();
        //     return default;
        // }
        //
        // public override T VisitTryGetItem(BoundTryGetItem x)
        // {
        //     // array, index, fallback
        //     base.VisitTryGetItem(x);
        //
        //     // TODO: resulting type if possible (see VisitArrayItem)
        //
        //     // The result of array[index] might be a reference
        //     x.TypeRefMask = TypeRefMask.AnyType.WithRefFlag;
        //
        //     return default;
        // }

        #endregion

        #region TypeRef

        //
        // public override T VisitIndirectTypeRef(BoundIndirectTypeRef tref)
        // {
        //     // visit indirect type
        //     base.VisitIndirectTypeRef(tref);
        //
        //     //
        //     return VisitTypeRef(tref);
        // }

        public override T VisitTypeRef(BoundTypeRef tref)
        {
            Debug.Assert(!(tref is BoundMultipleTypeRef));

            // resolve type symbol
            tref.ResolvedType = (TypeSymbol)tref.ResolveTypeSymbol(DeclaringCompilation);

            return default;
        }

        #endregion

        #region Visit Function Call

        public override T VisitInstanceCallEx(BoundInstanceCallEx x)
        {
            Visit(x.Instance, BoundAccess.Read);
            return base.VisitInstanceCallEx(x);
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
                    // var r = (TypeRefMask) 0;
                    // foreach (var m in ambiguity.Ambiguities)
                    // {
                    //     if (Worklist.EnqueueMethod(m, CurrentBlock, x))
                    //     {
                    //         // The next blocks will be analysed after this method is re-enqueued due to the dependency
                    //         _flags |= AnalysisFlags.IsCanceled;
                    //     }
                    //
                    //     r |= m.GetResultType(TypeCtx);
                    // }
                    //
                    // x.TypeRefMask = r;
                }
            }

            if (x.Access.IsReadRef)
            {
                // reading by ref:
                //x.TypeRefMask = x.TypeRefMask.WithRefFlag;
            }
        }

        // helper
        MethodSymbol[] Construct(MethodSymbol[] methods, BoundCallEx bound)
        {
            if (bound.TypeArguments.IsDefaultOrEmpty)
            {
                return methods;
            }
            else
            {
                var types = bound.TypeArguments.Select(t => (TypeSymbol)t.Type).AsImmutable();
                var result = new List<MethodSymbol>();

                for (int i = 0; i < methods.Length; i++)
                {
                    if (methods[i].Arity == types.Length) // TODO: check the type argument is assignable
                    {
                        result.Add(methods[i].Construct(types));
                    }
                }

                return result.ToArray();
            }
        }
        //
        // public override T VisitNew(BoundNewEx x)
        // {
        //     Accept(x.TypeRef); // resolve target type
        //
        //     VisitMethodCall(x); // analyse arguments
        //
        //     // resolve .ctor method:
        //     var type = (NamedTypeSymbol) x.TypeRef.Type;
        //     if (type.IsValidType())
        //     {
        //         var candidates = type.InstanceConstructors.ToArray();
        //
        //         //
        //         x.TargetMethod = new OverloadsList(candidates).Resolve(this.TypeCtx, x.ArgumentsInSourceOrder,
        //             VisibilityScope, OverloadsList.InvocationKindFlags.New);
        //         x.ResultType = type;
        //     }
        //
        //     // bind arguments:
        //     BindMethodCall(x);
        //
        //     // resulting type is always known,
        //     // not null,
        //     // not ref:
        //     x.TypeRefMask = x.TypeRef.GetTypeRefMask(TypeCtx).WithoutSubclasses;
        //
        //     return default;
        // }
        //
        // public override T VisitInclude(BoundIncludeEx x)
        // {
        //     VisitMethodCall(x);
        //
        //     // resolve target script
        //     Debug.Assert(x.ArgumentsInSourceOrder.Length == 1);
        //     var targetExpr = x.ArgumentsInSourceOrder[0].Value;
        //
        //     //
        //     x.TargetMethod = null;
        //
        //     if (targetExpr.ConstantValue.TryConvertToString(out var path))
        //     {
        //         // include (path)
        //         x.TargetMethod = (MethodSymbol) _model.ResolveFile(path)?.MainMethod;
        //     }
        //     else if (targetExpr is BoundConcatEx concat) // common case
        //     {
        //         // include (dirname( __FILE__ ) . path) // changed to (__DIR__ . path) by graph rewriter
        //         // include (__DIR__ . path)
        //         if (concat.ArgumentsInSourceOrder.Length == 2 &&
        //             // concat.ArgumentsInSourceOrder[0].Value is BoundPseudoConst pc &&
        //             // pc.ConstType == PseudoConstUse.Types.Dir &&
        //             concat.ArgumentsInSourceOrder[1].Value.ConstantValue.TryConvertToString(out path))
        //         {
        //             // create project relative path
        //             // not starting with a directory separator!
        //             path = Method.ContainingFile.DirectoryRelativePath + path;
        //             if (path.Length != 0 && PathUtilities.IsAnyDirectorySeparator(path[0]))
        //                 path = path.Substring(1); // make nicer when we have a helper method for that
        //             x.TargetMethod = (MethodSymbol) _model.ResolveFile(path)?.MainMethod;
        //         }
        //     }
        //
        //     // resolve result type
        //     if (x.Access.IsRead)
        //     {
        //         var target = x.TargetMethod;
        //         if (target != null)
        //         {
        //             x.ResultType = target.ReturnType;
        //             x.TypeRefMask = target.GetResultType(TypeCtx);
        //
        //             if (x.IsOnceSemantic)
        //             {
        //                 // include_once, require_once returns TRUE in case the script was already included
        //                 x.TypeRefMask |= TypeCtx.GetBooleanTypeMask();
        //             }
        //         }
        //         else
        //         {
        //             x.TypeRefMask = TypeRefMask.AnyType;
        //         }
        //     }
        //     else
        //     {
        //         x.TypeRefMask = 0;
        //     }
        //
        //     // reset type analysis (include may change local variables)
        //     State.SetAllUnknown(true);
        //
        //     return default;
        // }

        #endregion

        #region Visit FieldRef

        public override T VisitFieldRef(BoundFieldRef x)
        {
            Accept(x.Instance);
            Accept(x.ContainingType);
            Accept(x.FieldName);


            return default;
        }

        #endregion

        #region Visit ArrayEx, ArrayItemEx, ArrayItemOrdEx

        public override T VisitArrayEx(BoundArrayEx x)
        {
            // var items = x.Items;
            // TypeRefMask elementType = 0;
            //
            // // analyse elements
            // foreach (var i in items)
            // {
            //     Debug.Assert(i.Value != null);
            //
            //     Accept(i.Key);
            //     Accept(i.Value);
            //
            //     elementType |= i.Value.TypeRefMask;
            // }
            //
            // // writeup result type
            // x.TypeRefMask = elementType.IsVoid
            //     ? TypeCtx.GetArrayTypeMask()
            //     : TypeCtx.GetArrayTypeMask(elementType);

            return default;
        }

        public override T VisitArrayItemEx(BoundArrayItemEx x)
        {
            Accept(x.Array);
            Accept(x.Index);

            // TODO: resulting type if possible:
            // var element_type = TypeCtx.GetElementType(x.Array.TypeRefMask); // + handle classes with ArrayAccess and TypeRefMask.Uninitialized

            //

            // x.TypeRefMask =
            //     x.Access.IsReadRef ? TypeRefMask.AnyType.WithRefFlag :
            //     x.Access.IsEnsure ? TypeRefMask.AnyType : // object|array ?
            //     TypeRefMask.AnyType.WithRefFlag; // result might be a anything (including a reference?)

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

        // public override T VisitYieldEx(BoundYieldEx x)
        // {
        //     base.VisitYieldEx(x);
        //     x.TypeRefMask = TypeRefMask.AnyType;
        //
        //     return default;
        // }

        // public override T VisitYieldFromEx(BoundYieldFromEx x)
        // {
        //     base.VisitYieldFromEx(x);
        //     x.TypeRefMask = TypeRefMask.AnyType;
        //
        //     return default;
        // }

        #endregion

        #region Visit

        // public override T VisitIsEmpty(BoundIsEmptyEx x)
        // {
        //     Accept(x.Operand);
        //     x.TypeRefMask = TypeCtx.GetBooleanTypeMask();
        //
        //     return default;
        // }

        public override T VisitListEx(BoundListEx x)
        {
            // var elementtype = this.TypeCtx.GetElementType(x.Access.WriteMask);
            // Debug.Assert(!elementtype.IsVoid);
            //
            // foreach (var v in x.Items) // list() may contain NULL implying ignored variable
            // {
            //     if (v.Value != null)
            //     {
            //         Accept(v.Key);
            //         Visit(v.Value, v.Value.Access.WithWrite(elementtype));
            //     }
            // }

            return default;
        }

        public override T VisitConditionalEx(BoundConditionalEx x)
        {
            // BoundExpression positiveExpr; // positive expression (if evaluated to true, FalseExpr is not evaluated)
            // FlowState positiveState; // state after successful positive branch
            //
            // if (x.IfTrue != null && x.IfTrue != x.Condition)
            // {
            //     // Template: Condition ? IfTrue : IfFalse
            //
            //     var originalState = State.Clone();
            //     positiveExpr = x.IfTrue;
            //
            //     // true branch:
            //     if (VisitCondition(x.Condition, ConditionBranch.ToTrue))
            //     {
            //         Accept(x.IfTrue);
            //         positiveState = State;
            //
            //         // false branch
            //         State = originalState.Clone();
            //         VisitCondition(x.Condition, ConditionBranch.ToFalse);
            //     }
            //     else
            //     {
            //         // OPTIMIZATION: Condition does not have to be visited twice!
            //
            //         originalState = State.Clone(); // state after visiting Condition
            //
            //         Accept(x.IfTrue);
            //         positiveState = State;
            //
            //         State = originalState.Clone();
            //     }
            // }
            // else
            // {
            //     // Template: Condition ?: IfFalse
            //     positiveExpr = x.Condition;
            //
            //     // in case ?: do not evaluate trueExpr twice:
            //     // Template: Condition ?: FalseExpr
            //
            //     Accept(x.Condition);
            //     positiveState = State.Clone();
            //
            //     // condition != false => condition != null =>
            //     // ignoring NULL type from Condition:
            //     x.Condition.TypeRefMask = TypeCtx.WithoutNull(x.Condition.TypeRefMask);
            // }
            //
            // // and start over with false branch:
            // Accept(x.IfFalse);
            //
            // // merge both states (after positive evaluation and the false branch)
            // State = State.Merge(positiveState);
            // x.TypeRefMask = positiveExpr.TypeRefMask | x.IfFalse.TypeRefMask;

            return default;
        }

        public override T VisitExpressionStmt(BoundExpressionStmt x)
        {
            return base.VisitExpressionStmt(x);
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

        // public override T VisitEval(BoundEvalEx x)
        // {
        //     base.VisitEval(x);
        //
        //     //
        //     State.SetAllUnknown(true);
        //
        //     //
        //     x.TypeRefMask = TypeRefMask.AnyType;
        //
        //     return default;
        // }

        #endregion
    }
}