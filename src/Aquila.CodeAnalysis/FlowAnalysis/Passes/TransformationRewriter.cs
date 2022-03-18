using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Semantics.Graph;
using Aquila.Compiler.Utilities;
using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Source;
using Aquila.Syntax.Ast;
using Microsoft.CodeAnalysis.Operations;
using MoreLinq.Extensions;
using Aquila.CodeAnalysis.Utilities;


namespace Aquila.CodeAnalysis.FlowAnalysis.Passes
{
    internal partial class TransformationRewriter : GraphRewriter
    {
        private readonly DelayedTransformations _delayedTransformations;
        private readonly SourceMethodSymbol _method;
        private readonly HashSet<BoundCopyValue> _unnecessaryCopies; // Possibly null if all are necessary

        protected AquilaCompilation DeclaringCompilation => _method.DeclaringCompilation;
        protected PrimitiveBoundTypeRefs PrimitiveBoundTypeRefs => DeclaringCompilation.TypeRefs;

        public int TransformationCount { get; private set; }

        public static bool TryTransform(DelayedTransformations delayedTransformations, SourceMethodSymbol method)
        {
            if (method.ControlFlowGraph == null)
            {
                // abstract method
                return false;
            }

            //
            var rewriter = new TransformationRewriter(delayedTransformations, method);
            var currentCFG = method.ControlFlowGraph;
            var updatedCFG = (ControlFlowGraph)rewriter.VisitCFG(currentCFG);

            rewriter.TryTransformParameters();
            method.ControlFlowGraph = updatedCFG;

            Debug.Assert(updatedCFG == currentCFG ||
                         rewriter.TransformationCount !=
                         0); // cfg updated => transformations (not <= due to parameter transformations)
            return updatedCFG != currentCFG;
        }

        // /// <summary>
        // /// Map of well-known methods and corresponding rewrite rule that can return a new expression as a replacement for the method call.
        // /// Return <c>null</c> if the method was not rewritten.
        // /// </summary>
        // readonly Dictionary<QualifiedName, Func<BoundGlobalFunctionCall, BoundExpression>> _special_functions;

        private TransformationRewriter()
        {
        }

        private TransformationRewriter(DelayedTransformations delayedTransformations, SourceMethodSymbol method)
            : this()
        {
            _delayedTransformations = delayedTransformations;
            _method = method ?? throw ExceptionUtilities.ArgumentNull(nameof(method));

            // Gather information about value copy operations which can be removed
            _unnecessaryCopies = CopyAnalysis.TryGetUnnecessaryCopies(_method);
        }

        private void TryTransformParameters()
        {
            var needPassValueParams = ParameterAnalysis.GetNeedPassValueParams(_method);

            foreach (var parameter in _method.SourceParameters)
            {
                var varindex =
                    _method.ControlFlowGraph.FlowContext.GetVarIndex(
                        new VariableName(parameter.Syntax.Identifier.Text));
                if (!needPassValueParams.Get(varindex) && parameter.CopyOnPass)
                {
                    // It is unnecessary to copy a parameter whose value is only passed to another methods and cannot change
                    parameter.CopyOnPass = false;
                    TransformationCount++;
                }
            }
        }

        protected override void OnVisitCFG(ControlFlowGraph x)
        {
            Debug.Assert(_method.ControlFlowGraph == x);
        }

        private protected override void OnUnreachableMethodFound(SourceMethodSymbol method)
        {
            _delayedTransformations.UnreachableMethods.Add(method);
        }

        // private protected override void OnUnreachableTypeFound(SourceTypeSymbol type)
        // {
        //     _delayedTransformations.UnreachableTypes.Add(type);
        // }

        public override object VisitConditionalEx(BoundConditionalEx x)
        {
            x = (BoundConditionalEx)base.VisitConditionalEx(x);

            if (x.IfTrue != null) // otherwise it is (A ?: B) operator
            {
                if (x.Condition.ConstantValue.TryConvertToBool(out var condVal))
                {
                    TransformationCount++;
                    return (condVal ? x.IfTrue : x.IfFalse).WithAccess(x);
                }

                if (x.IfTrue.ConstantValue.IsBool(out bool trueVal) &&
                    x.IfFalse.ConstantValue.IsBool(out bool falseVal))
                {
                    if (trueVal && !falseVal)
                    {
                        // A ? true : false => (bool)A
                        TransformationCount++;
                        return new BoundConversionEx(x.Condition, PrimitiveBoundTypeRefs.BoolTypeRef,
                            PrimitiveBoundTypeRefs.BoolTypeRef.ResolvedType).WithAccess(x);
                    }
                    else if (!trueVal && falseVal)
                    {
                        // A ? false : true => !A
                        TransformationCount++;
                        return new BoundUnaryEx(x.Condition, Operations.LogicNegation, x.Condition.ResultType)
                            .WithAccess(x);
                    }
                }
            }

            return x;
        }

        public override object VisitBinaryEx(BoundBinaryEx x)
        {
            if (x.Operation == Operations.And ||
                x.Operation == Operations.Or)
            {
                // AND, OR:
                if (x.Left.ConstantValue.TryConvertToBool(out var bleft))
                {
                    if (x.Operation == Operations.And)
                    {
                        TransformationCount++;
                        // TRUE && Right => Right
                        // FALSE && Right => FALSE
                        return bleft ? x.Right : x.Left;
                    }
                    else if (x.Operation == Operations.Or)
                    {
                        TransformationCount++;
                        // TRUE || Right => TRUE
                        // FALSE || Right => Right
                        return bleft ? x.Left : x.Right;
                    }
                }

                if (x.Right.ConstantValue.TryConvertToBool(out var bright))
                {
                    if (x.Operation == Operations.And && bright == true)
                    {
                        TransformationCount++;
                        return x.Left; // Left && TRUE => Left
                    }
                    else if (x.Operation == Operations.Or && bright == false)
                    {
                        TransformationCount++;
                        // Left || FALSE => Left
                        return x.Left;
                    }
                }
            }
            else if (x.Operation == Operations.Mul)
            {
                if ((x.Left.ConstantValue.TryConvertToLong(out long leftCons) && leftCons == -1)
                    || (x.Right.ConstantValue.TryConvertToLong(out long rightCons) && rightCons == -1))
                {
                    // X * -1, -1 * X -> -X
                    TransformationCount++;
                    var expr = leftCons == -1 ? x.Right : x.Left;
                    return new BoundUnaryEx(expr, Operations.Minus, expr.ResultType).WithAccess(
                        x.Access);
                }
            }

            //
            return base.VisitBinaryEx(x);
        }

        public override object VisitUnaryEx(BoundUnaryEx x)
        {
            if (x.Operation == Operations.LogicNegation &&
                x.Operand is BoundUnaryEx ux &&
                ux.Operation == Operations.LogicNegation)
            {
                // !!X -> (bool)X
                TransformationCount++;
                return new BoundConversionEx((BoundExpression)Accept(ux.Operand), PrimitiveBoundTypeRefs.BoolTypeRef,
                        PrimitiveBoundTypeRefs.BoolTypeRef.ResolveTypeSymbol(_method.DeclaringCompilation))
                    .WithAccess(x.Access);
            }

            return base.VisitUnaryEx(x);
        }

        // public override object VisitCopyValue(BoundCopyValue x)
        // {
        //     var valueEx = (BoundExpression) Accept(x.Expression);
        //     if (valueEx.IsDeeplyCopied && _unnecessaryCopies?.Contains(x) != true)
        //     {
        //         return x.Update(valueEx);
        //     }
        //     else
        //     {
        //         // deep copy is unnecessary:
        //         TransformationCount++;
        //         return valueEx;
        //     }
        // }

        public override object VisitAssignEx(BoundAssignEx x)
        {
            return base.VisitAssignEx(x);
        }

        public override object VisitCFGConditionalEdge(ConditionalEdge x)
        {
            if (x.Condition.ConstantValue.TryConvertToBool(out bool condValue))
            {
                TransformationCount++;
                NotePossiblyUnreachable(condValue ? x.FalseTarget : x.TrueTarget);
                var target = condValue ? x.TrueTarget : x.FalseTarget;
                return new SimpleEdge((BoundBlock)Accept(target));
            }

            if (x.Condition is BoundBinaryEx bex)
            {
                // if (A && FALSE)
                if (bex.Operation == Operations.And && bex.Right.ConstantValue.TryConvertToBool(out var bright) &&
                    bright == false)
                {
                    // if (Left && FALSE) {Unreachable} else {F} -> if (Left) {F} else {F}
                    // result is always FALSE but we have to evaluate Left
                    TransformationCount++;
                    NotePossiblyUnreachable(x.TrueTarget);

                    var target = (BoundBlock)Accept(x.FalseTarget);
                    return new ConditionalEdge(target, target, bex.Left.WithAccess(BoundAccess.None));
                }

                // if (A || TRUE)
                if (bex.Operation == Operations.Or && bex.Right.ConstantValue.TryConvertToBool(out bright) &&
                    bright == true)
                {
                    // if (Left || TRUE) {T} else {Unreachable} -> if (Left) {T} else {T}
                    // result is always FALSE but we have to evaluate Left
                    TransformationCount++;
                    NotePossiblyUnreachable(x.FalseTarget);

                    var target = (BoundBlock)Accept(x.TrueTarget);
                    return new ConditionalEdge(target, target, bex.Left.WithAccess(BoundAccess.None));
                }
            }

            //
            return base.VisitCFGConditionalEdge(x);
        }

        // public override object VisitGlobalFunctionCall(BoundGlobalFunctionCall x)
        // {
        //     // first rewrite function arguments if possible
        //     var result = base.VisitGlobalFunctionCall(x);
        //
        //     // use rewrite rule for this method:
        //     if ((x = result as BoundGlobalFunctionCall) != null && x.TargetMethod != null &&
        //         _special_functions.TryGetValue(new QualifiedName(new Name(x.TargetMethod.MethodName)),
        //             out var rewrite_func))
        //     {
        //         var newnode = rewrite_func(x);
        //         if (newnode != null)
        //         {
        //             TransformationCount++;
        //             return newnode;
        //         }
        //     }
        //
        //     //
        //     return result;
        // }
        //
        // public override object VisitConcat(BoundConcatEx x)
        // {
        //     // transform arguments first:
        //     x = (BoundConcatEx) base.VisitConcat(x);
        //
        //     //
        //     var args = x.ArgumentsInSourceOrder;
        //     if (args.Length == 0 || args.All(IsEmptyString))
        //     {
        //         // empty string:
        //         TransformationCount++;
        //         return new BoundLiteral(string.Empty)
        //             {ConstantValue = new Optional<object>(string.Empty)}.WithContext(x);
        //     }
        //
        //     // visit & concat in compile time if we can:
        //     var newargs = args;
        //     int i = 0;
        //     do
        //     {
        //         // accumulate evaluated string value if possible:
        //         if (newargs[i].Value.ConstantValue.TryConvertToString(out var value))
        //         {
        //             string result = value;
        //             int end = i + 1;
        //             while (end < newargs.Length && newargs[end].Value.ConstantValue.TryConvertToString(out var tmp))
        //             {
        //                 result += tmp;
        //                 end++;
        //             }
        //
        //             if (end > i + 1) // we concat'ed something!
        //             {
        //                 newargs = newargs.RemoveRange(i, end - i);
        //
        //                 if (!string.IsNullOrEmpty(result))
        //                 {
        //                     newargs = newargs.Insert(i, BoundArgument.Create(new BoundLiteral(result)
        //                     {
        //                         ConstantValue = new Optional<object>(result),
        //                         TypeRefMask = _method.TypeRefContext.GetStringTypeMask(),
        //                         ResultType = DeclaringCompilation.CoreTypes.String,
        //                     }.WithAccess(BoundAccess.Read)));
        //                 }
        //             }
        //         }
        //
        //         //
        //         i++;
        //     } while (i < newargs.Length);
        //
        //     //
        //     if (newargs != args)
        //     {
        //         TransformationCount++;
        //
        //         if (newargs.Length == 0)
        //         {
        //             return new BoundLiteral(string.Empty) {ConstantValue = new Optional<object>(string.Empty)}
        //                 .WithContext(x);
        //         }
        //         else if (newargs.Length == 1 && newargs[0].Value.ConstantValue.TryConvertToString(out var value))
        //         {
        //             // "value"
        //             return new BoundLiteral(value) {ConstantValue = new Optional<object>(value)}.WithContext(x);
        //         }
        //
        //         //
        //         return x.Update(newargs);
        //     }
        //
        //     //
        //     return x;
        // }

        public override object VisitLiteral(BoundLiteral x)
        {
            // implicit conversion: string -> callable
            if (x.Access.TargetType == null &&
                x.ConstantValue.TryConvertToString(out var fnName))
            {
                // Template: (callable)"fnName"
                // resolve the 'MethodInfo' if possible

                MethodSymbol symbol = null;
                var dc = fnName.IndexOf(Name.ClassMemberSeparator, StringComparison.Ordinal);
                if (dc < 0)
                {
                    var fnQName = NameUtils.MakeQualifiedName(fnName, true);
                    //symbol = (MethodSymbol) DeclaringCompilation.ResolveFunction(fnQName, _method);
                }
                else
                {
                    //var tname = fnName.Remove(dc);
                    //var mname = fnName.Substring(dc + Name.ClassMemberSeparator.Length);

                    // type::method
                    // self::method
                    // parent::method
                }

                if (symbol.IsValidMethod() ||
                    (symbol is AmbiguousMethodSymbol a && a.IsOverloadable && a.Ambiguities.Length != 0)
                   ) // valid or ambiguous
                {
                    throw new NotImplementedException();
                }
            }

            //
            return base.VisitLiteral(x);
        }

        public override object VisitMethodDeclStmt(BoundMethodDeclStmt x)
        {
            if (false && !IsConditional && _method.IsGlobalScope)
            {
                //_delayedTransformations.FunctionsMarkedAsUnconditional.Add(x.Method);
            }

            // no transformation
            return base.VisitMethodDeclStmt(x);
        }

        // public override object VisitIsEmpty(BoundIsEmptyEx x)
        // {
        //     if (x.Operand is BoundVariableRef varRef && varRef.Name.IsDirect && !varRef.Name.NameValue.IsAutoGlobal)
        //     {
        //         var flowContext = _method.ControlFlowGraph.FlowContext;
        //
        //         var varType = flowContext.GetVarType(varRef.Name.NameValue);
        //         if (!varType.IsRef && (varType.IsVoid || flowContext.TypeRefContext.WithoutNull(varType).IsVoid)
        //                            && !_method.IsGlobalScope &&
        //                            (_method.Flags & MethodFlags.RequiresLocalsArray) == 0)
        //         {
        //             // empty($x) where $x is undefined or null -> TRUE
        //             TransformationCount++;
        //             return new BoundLiteral(true.AsObject()).WithAccess(x.Access);
        //         }
        //     }
        //
        //     return base.VisitIsEmpty(x);
        // }

        public override object VisitExpressionStmt(BoundExpressionStmt x)
        {
            // Transform the original expression first
            x = (BoundExpressionStmt)base.VisitExpressionStmt(x);
            return x;
        }

        public override object VisitArrayEx(BoundArrayEx x)
        {
            bool TryGetMethod(TypeSymbol typeSymbol, string methodName, out MethodSymbol methodSymbol)
            {
                if (typeSymbol != null && typeSymbol.TypeKind != TypeKind.Error &&
                    typeSymbol.LookupMethods(methodName).SingleOrDefault() is MethodSymbol mSymbol &&
                    mSymbol.IsValidMethod() &&
                    mSymbol.IsAccessible(_method.ContainingType))
                {
                    methodSymbol = mSymbol;
                    return true;
                }
                else
                {
                    methodSymbol = null;
                    return false;
                }
            }

            // implicit conversion: ["typeName" / $this, "methodName"] -> callable
            if (x.Access.TargetType == null &&
                x.Items.Length == 2 && x.Items[1].Value.ConstantValue.TryConvertToString(out var methodName))
            {
                var item0 = x.Items[0].Value;
                if (item0.ConstantValue.TryConvertToString(out var typeName))
                {
                    var typeQName = NameUtils.MakeQualifiedName(typeName, true);
                    TypeSymbol typeSymbol = null;
                    if (typeQName.IsReservedClassName)
                    {
                        //assign typeSymbol
                        throw new NotImplementedException();
                    }
                    else
                    {
                        typeSymbol = DeclaringCompilation.GlobalSemantics.ResolveType(typeQName) as TypeSymbol;
                    }

                    if (TryGetMethod(typeSymbol, methodName, out var methodSymbol) && methodSymbol.IsStatic)
                    {
                        throw new NotImplementedException();
                    }
                }
            }

            return base.VisitArrayEx(x);
        }

        /// <summary>
        /// If <paramref name="expr"/> is of type <typeparamref name="T"/> or it is a <see cref="BoundCopyValue" /> enclosing an
        /// expression of type <typeparamref name="T"/>, store the expression to <paramref name="typedExpr"/> and return true;
        /// otherwise, return false. Store to <paramref name="isCopied"/> whether <paramref name="typedExpr"/> was enclosed in
        /// <see cref="BoundCopyValue"/>.
        /// </summary>
        private static bool MatchExprSkipCopy<T>(BoundExpression expr, out T typedExpr, out bool isCopied)
            where T : BoundExpression
        {
            if (expr is T res)
            {
                typedExpr = res;
                isCopied = false;
                return true;
            }
            else if (expr is BoundCopyValue copyVal)
            {
                isCopied = true;
                return MatchExprSkipCopy<T>(copyVal.Expression, out typedExpr, out _);
            }
            else
            {
                typedExpr = default;
                isCopied = false;
                return false;
            }
        }

        private static bool IsEmptyString(BoundArgument a) => a.Value.ConstantValue.HasValue &&
                                                              ExpressionsExtension.IsEmptyStringValue(
                                                                  a.Value.ConstantValue.Value);
    }
}