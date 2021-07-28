using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Transactions;
using Aquila.CodeAnalysis.Errors;
using Aquila.CodeAnalysis.FlowAnalysis;
using Aquila.CodeAnalysis.Semantics.Model;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Utilities;
using Aquila.Syntax;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Ast.Expressions;
using Aquila.Syntax.Ast.Functions;
using Aquila.Syntax.Ast.Statements;
using Aquila.Syntax.Declarations;
using Aquila.Syntax.Errors;
using Aquila.Syntax.Text;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Semantics
{
    internal class BinderFactory : AstWalker<Binder>
    {
        private readonly Dictionary<LangElement, Binder> _resolvedBinders;
        private Stack<Binder> _stack;

        private Binder TopStack
        {
            get => _stack.TryPeek(out var res) ? res : null;
        }

        private MergedSourceUnit _merged;
        private readonly AquilaCompilation _compilation;

        public BinderFactory(MergedSourceUnit merged, AquilaCompilation compilation)
        {
            _merged = merged;
            _compilation = compilation;
            _stack = new Stack<Binder>();

            _stack.Push(new GlobalBinder());
        }

        public void Push(Binder binder)
        {
            Debug.Assert(binder != null);
            _stack.Push(binder);
        }

        public Binder Pop()
        {
            return _stack.Pop();
        }

        public Binder CreateBinder(LangElement element)
        {
            if (_resolvedBinders.TryGetValue(element, out var binder))
                return binder;

            binder = Visit(element);
            _resolvedBinders.Add(element, binder);

            return binder;
        }

        public override Binder VisitSourceUnit(SourceUnit arg)
        {
            var unitBinder = new Binder(TopStack);
            _resolvedBinders.Add(arg, unitBinder);

            Push(unitBinder);

            base.VisitSourceUnit(arg);

            Pop();

            return unitBinder;
        }

        public override Binder VisitComponentDecl(ComponentDecl arg)
        {
            return base.VisitComponentDecl(arg);
        }

        public override Binder VisitExtendDecl(ExtendDecl arg)
        {
            return base.VisitExtendDecl(arg);
        }
    }

    internal class InContainerBinder : Binder
    {
        private readonly NamedTypeSymbol _container;

        public InContainerBinder(NamedTypeSymbol container, Binder next) : base(next)
        {
            _container = container;
        }

        public override NamespaceOrTypeSymbol Container => _container;
    }

    internal class InMethodBinder : Binder
    {
        private readonly SourceMethodSymbol _method;
        private LocalsTable _locals;

        public InMethodBinder(SourceMethodSymbol method, Binder next) : base(next)
        {
            _method = method;
        }

        public override SourceMethodSymbol Method => _method;

        public override NamespaceOrTypeSymbol Container => _method.ContainingType;
    }

    internal class Binder
    {
        private readonly Binder _next;

        public Binder(Binder next)
        {
            _next = next;
        }

        public virtual Binder GetNext()
        {
            return _next;
        }

        public virtual Binder GetBinder(LangElement syntax)
        {
            return GetNext().GetBinder(syntax);
        }

        public DiagnosticBag Diagnostics => Container.DeclaringCompilation.DeclarationDiagnostics;

        public virtual SourceMethodSymbol Method { get; }
        public virtual NamespaceOrTypeSymbol Container { get; }

        protected virtual BoundStatement BindStatement(Statement stmt) => BindStatementCore(stmt).WithSyntax(stmt);

        BoundStatement BindStatementCore(Statement stmt)
        {
            Debug.Assert(stmt != null);

            if (stmt is ExpressionStmt exprStm)
                return new BoundExpressionStmt(BindExpression(exprStm.Expression, BoundAccess.None));
            if (stmt is ReturnStmt jmpStm) return BindReturnStmt(jmpStm);

            Diagnostics.Add(GetLocation(stmt), ErrorCode.ERR_NotYetImplemented,
                $"Statement of type '{stmt.GetType().Name}'");
            return new BoundEmptyStmt(stmt.Span.ToTextSpan());
        }

        protected virtual BoundExpression BindExpression(Expression expr, BoundAccess access) =>
            BindExpressionCore(expr, access).WithSyntax(expr);

        protected BoundExpression BindExpressionCore(Expression expr, BoundAccess access)
        {
            Debug.Assert(expr != null);

            if (expr is LiteralEx) return BindLiteral((LiteralEx)expr).WithAccess(access);
            if (expr is NameEx ne) return BindSimpleVarUse(ne, access);

            if (expr is BinaryEx bex) return BindBinaryEx(bex).WithAccess(access);
            if (expr is AssignEx aex) return BindAssignEx(aex, access);
            if (expr is UnaryEx ue) return BindUnaryEx(ue, access).WithAccess(access);
            if (expr is IncDecEx incDec) return BindIncDec(incDec).WithAccess(access);
            if (expr is CallEx ce) return BindCallEx(ce).WithAccess(access);
            if (expr is MemberAccessEx mae) return BindMemberAccessEx(mae, false).WithAccess(access);

            if (expr is ThrowEx throwEx)
                return new BoundThrowEx(BindExpression(throwEx.Expression, BoundAccess.Read));

            //
            Diagnostics.Add(GetLocation(expr), ErrorCode.ERR_NotYetImplemented,
                $"Expression of type '{expr.GetType().Name}'");
            return new BoundLiteral(null);
        }


        protected BoundExpression BindLiteral(LiteralEx expr)
        {
            switch (expr.Operation)
            {
                case Operations.IntLiteral:
                case Operations.StringLiteral:
                case Operations.DoubleLiteral:
                case Operations.NullLiteral:
                case Operations.BinaryStringLiteral:
                case Operations.BoolLiteral:
                    return new BoundLiteral(expr.ObjectiveValue);
                default:
                    throw new NotImplementedException();
            }
        }

        protected BoundExpression BindSimpleVarUse(NameEx expr, BoundAccess access)
        {
            var varname = BindVariableName(expr);

            if (true)
            {
                if (Method == null)
                {
                    // cannot use variables in field initializer or parameter initializer
                    Diagnostics.Add(GetLocation(expr), ErrorCode.ERR_InvalidConstantExpression);
                }

                if (varname.IsDirect)
                {
                    if (varname.NameValue.IsThisVariableName)
                    {
                        // $this is read-only
                        if (access.IsEnsure)
                            access = BoundAccess.Read;

                        if (access.IsWrite || access.IsUnset)
                            Diagnostics.Add(GetLocation(expr), ErrorCode.ERR_CannotAssignToThis);

                        // $this is only valid in global code and instance methods:
                        if (Method != null && Method.IsStatic && !Method.IsGlobalScope)
                        {
                            // WARN:
                            Diagnostics.Add(DiagnosticBagExtensions.ParserDiagnostic(ContainingFile, expr.Span,
                                Warnings.ThisOutOfMethod));
                            // ERR: // NOTE: causes a lot of project to not compile // CONSIDER: uncomment
                            // Diagnostics.Add(GetLocation(expr), Errors.ErrorCode.ERR_ThisOutOfObjectContext);
                        }
                    }
                }
                else
                {
                    if (Method != null)
                        Method.Flags |= MethodFlags.HasIndirectVar;
                }

                return new BoundVariableRef(varname).WithAccess(access);
            }
            else
            {
                var instanceAccess = BoundAccess.Read;

                if (access.IsWrite || access.EnsureObject || access.EnsureArray)
                    instanceAccess = instanceAccess.WithEnsureObject();

                if (access.IsQuiet)
                    instanceAccess = instanceAccess.WithQuiet();

                return BoundFieldRef
                    .CreateInstanceField(BindExpression(null, instanceAccess), varname)
                    .WithAccess(access);
            }
        }

        protected BoundVariableName BindVariableName(NameEx nameEx)
        {
            Debug.Assert(nameEx != null);

            return new BoundVariableName(nameEx.Identifier.Text).WithSyntax(nameEx);
        }

        protected Location GetLocation(LangElement expr) => expr.SyntaxTree.GetLocation(expr);

        protected virtual BoundExpression BindBinaryEx(BinaryEx expr)
        {
            BoundAccess laccess = BoundAccess.Read;

            switch (expr.Operation)
            {
                case Operations.Concat: // Left . Right
                    throw new NotSupportedException(
                        "Concat operation not supported"); //return BindConcatEx(new[] {expr.Left, expr.Right});

                case Operations.Coalesce:
                    laccess = BoundAccess.Read.WithQuiet(); // Template: A ?? B; // read A quietly
                    goto default;

                default:
                    return new BoundBinaryEx(
                        BindExpression(expr.Left, laccess),
                        BindExpression(expr.Right, BoundAccess.Read),
                        expr.Operation);
            }
        }

        protected BoundExpression BindUnaryEx(UnaryEx expr, BoundAccess access)
        {
            var operandAccess = BoundAccess.Read;

            switch (expr.Operation)
            {
                case Operations.AtSign:
                    operandAccess = access; // TODO: | Quiet
                    break;
                case Operations.UnsetCast:
                    operandAccess = BoundAccess.None;
                    break;
            }

            var boundOperation = BindExpression(expr.Expression, operandAccess);

            switch (expr.Operation)
            {
                case Operations.BoolCast:
                    return new BoundConversionEx(boundOperation, BoundTypeRefFactory.BoolTypeRef);

                case Operations.Int8Cast:
                case Operations.Int16Cast:
                case Operations.Int32Cast:
                case Operations.UInt8Cast:
                case Operations.UInt16Cast:
                // -->
                case Operations.UInt64Cast:
                case Operations.UInt32Cast:
                case Operations.Int64Cast:
                    return new BoundConversionEx(boundOperation, BoundTypeRefFactory.LongTypeRef);

                case Operations.DecimalCast:
                case Operations.DoubleCast:
                case Operations.FloatCast:
                    return new BoundConversionEx(boundOperation, BoundTypeRefFactory.DoubleTypeRef);

                case Operations.UnicodeCast:
                    return new BoundConversionEx(boundOperation, BoundTypeRefFactory.StringTypeRef);

                case Operations.StringCast:

                    // // workaround (binary) is parsed as (StringCast)
                    // if (expr.ContainingSourceUnit.GetSourceCode(expr.Span).StartsWith("(binary)"))
                    // {
                    //     goto case Operations.BinaryCast;
                    // }

                    return
                        new BoundConversionEx(boundOperation,
                            BoundTypeRefFactory
                                .StringTypeRef); // TODO // CONSIDER: should be WritableString and analysis should rewrite it to String if possible

                case Operations.BinaryCast:
                    return new BoundConversionEx(
                        new BoundConversionEx(
                            boundOperation,
                            BoundTypeRefFactory.Create(
                                DeclaringCompilation.CreateArrayTypeSymbol(
                                    DeclaringCompilation.GetSpecialType(SpecialType.System_Byte)))
                        ).WithAccess(BoundAccess.Read),
                        BoundTypeRefFactory.WritableStringRef);

                case Operations.ArrayCast:
                    return new BoundConversionEx(boundOperation, BoundTypeRefFactory.ArrayTypeRef);

                case Operations.ObjectCast:
                    return new BoundConversionEx(boundOperation, BoundTypeRefFactory.ObjectTypeRef);

                default:
                    return new BoundUnaryEx(BindExpression(expr.Expression, operandAccess), expr.Operation);
            }
        }

        protected BoundExpression BindAssignEx(AssignEx expr, BoundAccess access)
        {
            var target = (BoundReferenceEx)BindExpression(expr.LValue, BoundAccess.Write);
            BoundExpression value;

            // bind value (read as value or as ref)
            if (expr is AssignEx assignEx)
            {
                value = BindExpression(assignEx.RValue, BoundAccess.Read);

                // we don't need copy of RValue if assigning to list() or in a part of compound operation
                if (expr.Operation == Operations.AssignValue && !(target is BoundListEx))
                {
                    value = BindCopyValue(value);
                }
            }
            // else if (expr is RefAssignEx refAssignEx)
            // {
            //     Debug.Assert(expr.Operation == Operations.AssignRef);
            //     target.Access = target.Access.WithWriteRef(0); // note: analysis will write the write type
            //     value = BindExpression(refAssignEx.RValue, BoundAccess.ReadRef);
            // }
            else
            {
                ExceptionUtilities.UnexpectedValue(expr);
                return null;
            }

            //
            if (expr.Operation == Operations.AssignValue || expr.Operation == Operations.AssignRef)
            {
                return new BoundAssignEx(target, value).WithAccess(access);
            }
            else
            {
                if (target is BoundArrayItemEx itemex && itemex.Index == null)
                {
                    // Special case:
                    switch (expr.Operation)
                    {
                        // "ARRAY[] .= VALUE" => "ARRAY[] = (string)VALUE"
                        case Operations.AssignPrepend:
                        case Operations.AssignAppend: // .=
                            // value = BindConcatEx(new List<BoundArgument>()
                            // {
                            //     BoundArgument.Create(new BoundLiteral(null).WithAccess(BoundAccess.Read)),
                            //     BoundArgument.Create(value)
                            // });
                            break;

                        default:
                            value = new BoundBinaryEx(new BoundLiteral(null).WithAccess(BoundAccess.Read), value,
                                AstUtils.CompoundOpToBinaryOp(expr.Operation));
                            break;
                    }

                    return new BoundAssignEx(target, value.WithAccess(BoundAccess.Read)).WithAccess(access);
                }
                else
                {
                    target.Access = target.Access.WithRead(); // Read & Write on target

                    return new BoundCompoundAssignEx(target, value, expr.Operation).WithAccess(access);
                }
            }
        }

        protected BoundExpression BindIncDec(IncDecEx expr)
        {
            // bind variable reference
            var varref = (BoundReferenceEx)BindExpression(expr.Operand, BoundAccess.ReadAndWrite);

            //
            return new BoundIncDecEx(varref, expr.IsIncrement, expr.IsPost);
        }

        private BoundExpression BindCallEx(CallEx expr)
        {
            return BindMethodGroup(expr.Expression, expr.Arguments, true);
        }

        private BoundExpression BindMethodGroup(Expression expr, ArgumentList args, bool invoked)
        {
            switch (expr.Kind)
            {
                case SyntaxKind.NameExpression:
                    return BindName((NameEx)expr, args, invoked);
                case SyntaxKind.MemberAccessExpression:
                    return BindMemberAccessEx((MemberAccessEx)expr, invoked);
                default:
                    throw new NotImplementedException();
            }
        }

        private BoundExpression BindName(NameEx expr, ArgumentList argumentList, bool invocation)
        {
            var members = this.Method.ContainingType.GetMembers(expr.Identifier.Text).Where(x => x is MethodSymbol)
                .ToList();

            if (!members.Any())
                Diagnostics.Add(GetLocation(expr), ErrorCode.ERR_NotYetImplemented);

            if (members.Count() == 1)
            {
                var member = members[0];
                if (invocation)
                {
                    if (member is MethodSymbol ms)
                    {
                        if (ms.IsStatic)
                        {
                            var arglist = argumentList.Select(x => BoundArgument.Create(BindExpression(x.Expression)))
                                .ToImmutableArray();

                            return new BoundStaticCallEx(ms,
                                new BoundMethodName(new QualifiedName(new Name(expr.Identifier.Text))),
                                arglist, ImmutableArray<IBoundTypeRef>.Empty);
                        }
                        else
                        {
                            var th = new BoundVariableRef("this");

                            return new BoundInstanceCallEx(ms,
                                new BoundMethodName(new QualifiedName(new Name(expr.Identifier.Text))),
                                ImmutableArray<BoundArgument>.Empty, ImmutableArray<IBoundTypeRef>.Empty, th
                            );
                        }
                    }
                }
                else
                {
                    if (member is MethodSymbol ms)
                    {
                        //... BoundMethod
                    }

                    if (member is FieldSymbol fs)
                    {
                        // ld_fld
                    }

                    if (member is PropertySymbol ps)
                    {
                        // call _get method
                    }
                }
            }

            if (members.Count() > 1 && invocation)
            {
                throw new NotImplementedException("");
                // MethodGroup
            }
            else
            {
                Diagnostics.Add(GetLocation(expr), ErrorCode.ERR_NotYetImplemented);
                return null;
            }
        }

        private BoundExpression BindMemberAccessEx(MemberAccessEx expr, bool invoke)
        {
            var boundLeft = BindExpression(expr.Expression);

            var leftType = boundLeft.Type;

            var members = leftType.GetMembers(expr.Identifier.Text);
            foreach (var member in members)
            {
                if (invoke)
                {
                    if (member is MethodSymbol ms)
                    {
                        //var method = new BoundMethod(ms, leftType);
                    }
                }
                else
                {
                    if (member is PropertySymbol ps)
                    {
                        throw new Exception("Can't bound property: not implemented");
                    }
                }
            }

            throw new NotImplementedException();
        }

        public BoundStatement BindEmptyStmt(Span span)
        {
            return new BoundEmptyStmt(span.ToTextSpan());
        }


        /*
Component
    - ComponentItem
        - Module
            -Code
                -Method
                
                
-global
    - import Reference;
    
    - static int global_function()
    
    - component Document
        
        - extend Invoice
            
            - void SetSum()
               {
                    Order d = new Order(); 
                        ^
                    This is type founded in "InNamespaceContainer"
                }   
                
Binder
    -> GetNext() - getting next Binder for the lookup
    -> Container - TypeOrNamespace Context
         */
    }

    internal class GlobalBinder : Binder
    {
        public GlobalBinder() : base(null)
        {
        }
    }
}