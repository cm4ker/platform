using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Aquila.CodeAnalysis.Errors;
using Aquila.CodeAnalysis.FlowAnalysis;
using Aquila.CodeAnalysis.Semantics.TypeRef;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Source;
using Aquila.CodeAnalysis.Utilities;
using Aquila.Compiler.Utilities;
using Aquila.Core.Querying.Model;
using Aquila.Syntax;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Ast.Expressions;
using Aquila.Syntax.Ast.Functions;
using Aquila.Syntax.Ast.Statements;
using Aquila.Syntax.Declarations;
using Aquila.Syntax.Errors;
using Aquila.Syntax.Syntax;
using Aquila.Syntax.Text;
using Microsoft.CodeAnalysis;
using MoreLinq.Extensions;
using EnumerableExtensions = Roslyn.Utilities.EnumerableExtensions;

namespace Aquila.CodeAnalysis.Semantics
{
    internal class BinderFactory : AstVisitorBase<Binder>
    {
        private readonly ConcurrentDictionary<LangElement, Binder> _resolvedBinders;
        private Stack<Binder> _stack;

        private Binder TopStack
        {
            get => _stack.TryPeek(out var res) ? res : null;
        }

        private MergedSourceUnit _merged;
        private readonly AquilaCompilation _compilation;
        private readonly GlobalBinder _global;

        public BinderFactory(AquilaCompilation compilation)
        {
            _compilation = compilation;
            _merged = compilation.SourceSymbolCollection.GetMergedUnit();

            _resolvedBinders = new ConcurrentDictionary<LangElement, Binder>();
            _stack = new Stack<Binder>();

            _global = new GlobalBinder(_compilation.GlobalNamespace);

            _stack.Push(_global);
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

        public Binder Peek()
        {
            return _stack.Peek();
        }

        /*
Component
    - ComponentItem
        - Module
            -Code
                -Method
                
                
-global <--- Binder here
    - import Reference; <--- Binder here
    
    - static int global_function()
    
    - component Document <---- Binder here
        
        - extend Invoice <---- Binder here
            
            - void SetSum() <---- Binder here
               {            
                    Order d = new Order(); 
                        ^
                    This is type founded in "InNamespaceContainer"
                    
                    { <---- Binder here on every scope created
                    
                    
                    }
                }   
                
Binder
    -> GetNext() - getting next Binder for the lookup
    -> Container - TypeOrNamespace Context
         */

        private Binder CreateBinder(LangElement element)
        {
            var next = Visit(element.Parent);

            if (element is ComponentDecl comDecl)
            {
                var ns = _compilation.PlatformSymbolCollection.GetNamespace(comDecl.Identifier.Text);

                var icBinder = new InContainerBinder(ns, next);
                _resolvedBinders.TryAdd(element, icBinder);

                return icBinder;
            }
            else if (element is ExtendDecl ext)
            {
                var types = next.Container.GetTypeMembers(ext.Identifier.Text);

                if (EnumerableExtensions.IsSingle(types))
                {
                    var icBinder = new InContainerBinder(types[0], next);
                    _resolvedBinders.TryAdd(element, icBinder);

                    return icBinder;
                }
            }
            else if (element is ImportDecl import)
            {
            }
            else if (element is MethodDecl md)
            {
                NamespaceOrTypeSymbol container;
                if (md.IsGlobal)
                    container = _compilation.SourceSymbolCollection.DefinedConstantsContainer;
                else
                    container = next.Container;

                var methods = container.GetMembers(md.Identifier.Text).OfType<SourceMethodSymbol>();
                var first = methods.First();
                var ib = new InMethodBinder(first, next);
                _resolvedBinders.TryAdd(element, ib);

                return ib;
            }
            else if (element is BlockStmt blc)
            {
                var b = new InMethodBinder(next.Method, next);
                _resolvedBinders.TryAdd(element, b);

                return b;
            }

            return next;
        }


        private void GetBinder(LangElement element, out Binder binder)
        {
            if (!_resolvedBinders.TryGetValue(element, out binder))
                binder = CreateBinder(element);
        }

        public override Binder VisitImportDecl(ImportDecl arg)
        {
            var nextBinder = Pop();

            var ns = _compilation.GlobalNamespace.GetMembers(arg.Name).ToImmutableArray();

            if (ns.Count() == 1)
            {
                var b = new InContainerBinder(ns.First(), nextBinder);
                _resolvedBinders.TryAdd(arg, b);

                return b;
            }

            throw new Exception("Namespace not found");
        }

        public override Binder VisitSourceUnit(SourceUnit arg)
        {
            Push(_global);
            if (arg.Imports.Any())
            {
                arg.Imports
                    .Reverse()
                    .Foreach(x =>
                    {
                        var b = Visit(x);
                        Push(b);
                    });
            }

            return Pop();
        }


        public override Binder VisitMethodDecl(MethodDecl arg)
        {
            GetBinder(arg, out var binder);
            return binder;
        }

        public override Binder VisitExtendDecl(ExtendDecl arg)
        {
            GetBinder(arg, out var binder);
            return binder;
        }

        public override Binder VisitComponentDecl(ComponentDecl arg)
        {
            GetBinder(arg, out var binder);
            return binder;
        }

        public override Binder DefaultVisit(LangElement node)
        {
            return Visit(node.Parent);
        }
    }

    internal class InContainerBinder : Binder
    {
        private readonly INamespaceOrTypeSymbol _container;

        public InContainerBinder(INamespaceOrTypeSymbol container, Binder next) : base(next)
        {
            _container = container;
        }

        public override NamespaceOrTypeSymbol Container => (NamespaceOrTypeSymbol)_container;


        protected override ITypeSymbol FindTypeByName(NamedTypeRef tref)
        {
            var qName = new QualifiedName(new Name(tref.Value));

            var typeMembers = Container.GetTypeMembers(qName.ToString(), -1);

            if (typeMembers.Length == 1)
                return typeMembers[0];

            if (typeMembers.Length == 0)
            {
                return GetNext().BindType(tref);
            }

            if (typeMembers.Length > 1)
            {
                Diagnostics.Add(GetLocation(tref), ErrorCode.WRN_UndefinedType,
                    $"Expression of type '{tref.GetType().Name}'");
            }

            return null;
        }
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

    internal class GlobalBinder : InContainerBinder
    {
        private readonly NamespaceOrTypeSymbol _ns;

        public GlobalBinder(INamespaceOrTypeSymbol ns) : base(ns, null)
        {
            _ns = (NamespaceOrTypeSymbol)ns;
        }

        public override NamespaceOrTypeSymbol Container => _ns;
    }


    abstract class Binder
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

        /// <summary>Gets <see cref="BoundTypeRefFactory"/> instance.</summary>
        internal BoundTypeRefFactory BoundTypeRefFactory => DeclaringCompilation.TypeRefFactory;

        public AquilaCompilation DeclaringCompilation => Container.DeclaringCompilation;


        public virtual SourceMethodSymbol Method { get; }
        public virtual NamespaceOrTypeSymbol Container { get; }
        public LocalsTable Locals => Method.LocalsTable;

        #region Bind statements

        public virtual BoundStatement BindStatement(Statement stmt) => BindStatementCore(stmt).WithSyntax(stmt);

        BoundStatement BindStatementCore(Statement stmt)
        {
            Debug.Assert(stmt != null);

            if (stmt is ExpressionStmt exprStm)
                return new BoundExpressionStmt(BindExpression(exprStm.Expression, BoundAccess.None));
            if (stmt is ReturnStmt jmpStm)
                return BindReturnStmt(jmpStm);
            if (stmt is VarDecl varDecl)
                return BindVarDeclStmt(varDecl);

            Diagnostics.Add(GetLocation(stmt), ErrorCode.ERR_NotYetImplemented,
                $"Statement of type '{stmt.GetType().Name}'");
            return new BoundEmptyStmt(stmt.Span.ToTextSpan());
        }

        private BoundStatement BindVarDeclStmt(VarDecl varDecl)
        {
            foreach (var decl in varDecl.Declarators)
            {
                var localVar = Method.LocalsTable.BindLocalVariable(new VariableName(decl.Identifier.Text), decl);
                var boundExpression = BindExpression(decl.Initializer);

                return new BoundExpressionStmt(new BoundAssignEx(
                    new BoundVariableRef(decl.Identifier.Text, localVar.Type).WithAccess(BoundAccess.Write),
                    boundExpression, localVar.Type));
                break;

                //TODO:
                //For not we support only one declarator. Need introduce Statements set for statements or change 
                //Result for IEnumerable<BoundStatement>
            }

            throw new Exception();
        }

        public TypeSymbol BindType(Aquila.Syntax.Ast.TypeRef tref)
        {
            var trf = DeclaringCompilation.CoreTypes;


            if (tref is PredefinedTypeRef pt)
            {
                switch (pt.Kind)
                {
                    case SyntaxKind.IntKeyword: return trf.Int32;
                    case SyntaxKind.LongKeyword: return trf.Int64;
                    case SyntaxKind.VoidKeyword: return trf.Void;
                    case SyntaxKind.ObjectKeyword: return trf.Object;
                    case SyntaxKind.StringKeyword: return trf.String;
                    case SyntaxKind.BoolKeyword: return trf.Boolean;

                    default: throw ExceptionUtilities.UnexpectedValue(pt.Kind);
                }
            }
            else if (tref is NamedTypeRef named)
            {
                var typeSymbol = FindTypeByName(named);
                return (TypeSymbol)typeSymbol;
            }
            else
            {
                Diagnostics.Add(GetLocation(tref), ErrorCode.ERR_NotYetImplemented,
                    $"Expression of type '{tref.GetType().Name}'");

                throw new NotImplementedException();
            }
        }

        protected virtual ITypeSymbol FindTypeByName(NamedTypeRef tref)
        {
            return _next?.FindTypeByName(tref);
        }

        protected BoundStatement BindReturnStmt(ReturnStmt stmt)
        {
            Debug.Assert(Method != null);

            BoundExpression expr = null;

            if (stmt.Expression != null)
            {
                expr = BindExpression(stmt.Expression, BoundAccess.Read);
            }

            return new BoundReturnStmt(expr).WithSyntax(expr.AquilaSyntax);
        }

        public BoundStatement BindEmptyStmt(Span span)
        {
            return new BoundEmptyStmt(span.ToTextSpan());
        }

        #endregion

        #region Bind expressions

        public virtual BoundExpression BindExpression(Expression expr, BoundAccess access) =>
            BindExpressionCore(expr, access).WithSyntax(expr);

        protected BoundExpression BindExpression(Expression expr) => BindExpression(expr, BoundAccess.Read);

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
                return new BoundThrowEx(BindExpression(throwEx.Expression, BoundAccess.Read), null);

            //
            Diagnostics.Add(GetLocation(expr), ErrorCode.ERR_NotYetImplemented,
                $"Expression of type '{expr.GetType().Name}'");
            return new BoundLiteral(null, null);
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
                    return new BoundLiteral(expr.ObjectiveValue, GetSymbolFromLiteralOperation(expr.Operation));

                case Operations.QueryLiteral:

                    var query = (string)expr.ObjectiveValue;
                    var element = QLang.Parse(query, DeclaringCompilation.MetadataCollection);
                    //TODO: Add errors to the DiagnosticBug if we catch error

                    return new BoundLiteral(expr.ObjectiveValue, GetSymbolFromLiteralOperation(expr.Operation));

                default:
                    throw new NotImplementedException();
            }
        }

        protected ITypeSymbol GetSymbolFromLiteralOperation(Operations op)
        {
            var ct = DeclaringCompilation.CoreTypes;

            return op switch
            {
                Operations.BoolLiteral => ct.Boolean.Symbol,
                Operations.IntLiteral => ct.Int32.Symbol,
                Operations.CharLiteral => ct.Char.Symbol,
                Operations.LongIntLiteral => ct.Int64.Symbol,
                Operations.DoubleLiteral => ct.Double.Symbol,
                Operations.StringLiteral => ct.String.Symbol,
                Operations.QueryLiteral => ct.String.Symbol,
                _ => new MissingMetadataTypeSymbol("Unknown", 1, false)
            };
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
                            Diagnostics.Add(DiagnosticBagExtensions.ParserDiagnostic(expr.SyntaxTree, expr.Span,
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

                return new BoundVariableRef(varname, varname.Type).WithAccess(access);
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
            Debug.Assert(Method != null);

            Method.LocalsTable.TryGetVariable(new VariableName(nameEx.Identifier.Text), out var localVar);
            var type = localVar.Type;
            return new BoundVariableName(nameEx.Identifier.Text, type).WithSyntax(nameEx);
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
                    var left = BindExpression(expr.Left, laccess);
                    var right = BindExpression(expr.Right, BoundAccess.Read);

                    return new BoundBinaryEx(left, right, expr.Operation, left.ResultType);
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
                    return new BoundConversionEx(boundOperation, BoundTypeRefFactory.BoolTypeRef, null);

                case Operations.Int8Cast:
                case Operations.Int16Cast:
                case Operations.Int32Cast:
                case Operations.UInt8Cast:
                case Operations.UInt16Cast:
                // -->
                case Operations.UInt64Cast:
                case Operations.UInt32Cast:
                case Operations.Int64Cast:
                    return new BoundConversionEx(boundOperation, BoundTypeRefFactory.LongTypeRef, null);

                case Operations.DecimalCast:
                case Operations.DoubleCast:
                case Operations.FloatCast:
                    return new BoundConversionEx(boundOperation, BoundTypeRefFactory.DoubleTypeRef, null);

                case Operations.UnicodeCast:
                    return new BoundConversionEx(boundOperation, BoundTypeRefFactory.StringTypeRef, null);

                case Operations.StringCast:

                    // // workaround (binary) is parsed as (StringCast)
                    // if (expr.ContainingSourceUnit.GetSourceCode(expr.Span).StartsWith("(binary)"))
                    // {
                    //     goto case Operations.BinaryCast;
                    // }

                    return
                        new BoundConversionEx(boundOperation,
                            BoundTypeRefFactory
                                .StringTypeRef, null);

                case Operations.BinaryCast:
                    return new BoundConversionEx(
                        new BoundConversionEx(
                            boundOperation,
                            BoundTypeRefFactory.Create(
                                DeclaringCompilation.CreateArrayTypeSymbol(
                                    DeclaringCompilation.GetSpecialType(SpecialType.System_Byte))), null
                        ).WithAccess(BoundAccess.Read),
                        BoundTypeRefFactory.StringTypeRef, null);

                // case Operations.ArrayCast:
                //     return new BoundConversionEx(boundOperation, BoundTypeRefFactory.ArrayTypeRef, null);

                case Operations.ObjectCast:
                    return new BoundConversionEx(boundOperation, BoundTypeRefFactory.ObjectTypeRef, null);

                default:
                    return new BoundUnaryEx(BindExpression(expr.Expression, operandAccess), expr.Operation, null);
            }
        }

        protected BoundExpression BindAssignEx(AssignEx expr, BoundAccess access)
        {
            var target = (BoundReferenceEx)BindExpression(expr.LValue, BoundAccess.Write);
            BoundExpression value;

            value = BindExpression(expr.RValue, BoundAccess.Read);

            if (expr.Operation == Operations.AssignValue && !(target is BoundListEx))
            {
                value = BindCopyValue(value);
            }

            if (expr.Operation == Operations.AssignValue || expr.Operation == Operations.AssignRef)
            {
                return new BoundAssignEx(target, value, value.ResultType).WithAccess(access);
            }
            else
            {
                if (target is BoundArrayItemEx itemex && itemex.Index == null)
                {
                    // Special case:
                    switch (expr.Operation)
                    {
                        case Operations.AssignPrepend:
                        case Operations.AssignAppend:
                            break;
                        default:
                            value = new BoundBinaryEx(new BoundLiteral(null, null).WithAccess(BoundAccess.Read), value,
                                AstUtils.CompoundOpToBinaryOp(expr.Operation), null);
                            break;
                    }

                    return new BoundAssignEx(target, value.WithAccess(BoundAccess.Read), value.ResultType)
                        .WithAccess(access);
                }
                else
                {
                    target.Access = target.Access.WithRead();

                    return new BoundCompoundAssignEx(target, value, expr.Operation, value.ResultType)
                        .WithAccess(access);
                }
            }
        }

        protected BoundExpression BindIncDec(IncDecEx expr)
        {
            // bind variable reference
            var varref = (BoundReferenceEx)BindExpression(expr.Operand, BoundAccess.ReadAndWrite);

            //
            return new BoundIncDecEx(varref, expr.IsIncrement, expr.IsPost, null);
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

        protected virtual BoundExpression BindName(NameEx expr, ArgumentList argumentList, bool invocation)
        {
            Debug.Assert(Method != null);

            var members = Container.GetMembers(expr.Identifier.Text).Where(x => x is MethodSymbol)
                .ToList();

            if (!Enumerable.Any(members))
                Diagnostics.Add(GetLocation(expr), ErrorCode.INF_CantResolveSymbol);

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
                                arglist, ImmutableArray<IBoundTypeRef>.Empty, null);
                        }
                        else
                        {
                            var type = Container as ITypeSymbol;

                            var th = new BoundVariableRef("this", type);

                            return new BoundInstanceCallEx(ms,
                                new BoundMethodName(new QualifiedName(new Name(expr.Identifier.Text))),
                                ImmutableArray<BoundArgument>.Empty, ImmutableArray<IBoundTypeRef>.Empty, th, null
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
                        return new BoundPropertyAccess(ps, boundLeft);
                    }
                }
            }

            throw new NotImplementedException();
        }

        protected BoundExpression BindCopyValue(BoundExpression expr)
        {
            if (expr.IsDeeplyCopied)
            {
                return new BoundCopyValue(expr).WithAccess(BoundAccess.Read);
            }
            else
            {
                // value does not have to be copied
                return expr;
            }
        }

        #endregion
    }


    /*
         
     public void some_method()
     {
        Query q = query();
        q.Text = "FROM A SELECT @Test as B";
        q.AddParameter("Test", "Value");  
     
     
        Reader r = q.exec();
        var was_break = false;
        
        while(r.read())
        {
            var value = r.B;
            //here var = string
            
            if(value == "Value")
            {
                message_to_client("Hello");
                log_error("Some value set to the \"Value\"");
                
                was_braked = true;
                break;
            }
        }
        
        if(was_break)
        {
            log_info("the cycle was break start create record in store");
            
            var store = new Entity.Store;
            
            store.init(was_break);
            store.is_main = false;
            var rows = store.rows;
            
            var first_row = rows.add();
        }
        
                
     }
     
     
     */
}