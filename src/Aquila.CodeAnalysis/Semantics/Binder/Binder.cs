using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Aquila.CodeAnalysis.Errors;
using Aquila.CodeAnalysis.FlowAnalysis;
using Aquila.CodeAnalysis.Semantics.TypeRef;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Syntax;
using Aquila.CodeAnalysis.Utilities;
using Aquila.Core.Querying.Model;
using Aquila.Syntax;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Declarations;
using Aquila.Syntax.Errors;
using Aquila.Syntax.Metadata;
using Aquila.Syntax.Syntax;
using Aquila.Syntax.Text;
using Microsoft.CodeAnalysis;
using EnumerableExtensions = Roslyn.Utilities.EnumerableExtensions;
using SpecialTypeExtensions = Aquila.CodeAnalysis.Symbols.SpecialTypeExtensions;

namespace Aquila.CodeAnalysis.Semantics
{
    internal class BinderFactory : AquilaSyntaxVisitor<Binder>
    {
        private readonly ConcurrentDictionary<AquilaSyntaxNode, Binder> _resolvedBinders;
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

            _resolvedBinders = new ConcurrentDictionary<AquilaSyntaxNode, Binder>();
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

        private Binder CreateBinder(AquilaSyntaxNode element)
        {
            var next = Visit(element.Parent);

            if (element is ComponentDecl comDecl)
            {
                var ns = _compilation.PlatformSymbolCollection.GetNamespace(comDecl.Name.GetUnqualifiedName().Identifier
                    .Text);

                var icBinder = new InContainerBinder(ns, next);
                _resolvedBinders.TryAdd(element, icBinder);

                return icBinder;
            }
            else if (element is ExtendDecl ext)
            {
                var types = next.Container.GetTypeMembers(ext.Name.GetUnqualifiedName().Identifier.Text);

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


        private void GetBinder(AquilaSyntaxNode element, out Binder binder)
        {
            if (!_resolvedBinders.TryGetValue(element, out binder))
                binder = CreateBinder(element);
        }

        public override Binder VisitImportDecl(ImportDecl arg)
        {
            var nextBinder = Pop();

            var ns = _compilation.GlobalNamespace.GetMembers(arg.Name.GetUnqualifiedName().Identifier.Text)
                .ToImmutableArray();

            if (ns.Count() == 1)
            {
                var b = new InContainerBinder(ns.First(), nextBinder);
                _resolvedBinders.TryAdd(arg, b);

                return b;
            }

            throw new Exception("Namespace not found");
        }

        public override Binder? VisitCompilationUnit(CompilationUnitSyntax node)
        {
            Push(_global);
            if (node.Imports.Any())
            {
                node.Imports
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

        public override Binder? DefaultVisit(SyntaxNode node)
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


        protected override ITypeSymbol FindTypeByName(NameEx tref)
        {
            var qName = tref.GetUnqualifiedName().Identifier.Text;

            var typeMembers = Container.GetTypeMembers(qName, -1);

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

        protected override IEnumerable<IMethodSymbol> FindMethodsByName(string name)
        {
            return _ns.GetMembers(name).OfType<MethodSymbol>();
        }
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

        public virtual Binder GetBinder(AquilaSyntaxNode syntax)
        {
            return GetNext().GetBinder(syntax);
        }

        public DiagnosticBag Diagnostics => Container.DeclaringCompilation.DeclarationDiagnostics;

        /// <summary>Gets <see cref="PrimitiveBoundTypeRefs"/> instance.</summary>
        internal PrimitiveBoundTypeRefs PrimitiveBoundTypeRefs => DeclaringCompilation.TypeRefs;

        public AquilaCompilation DeclaringCompilation => Container.DeclaringCompilation;


        public virtual SourceMethodSymbol Method { get; }
        public virtual NamespaceOrTypeSymbol Container { get; }
        public LocalsTable Locals => Method.LocalsTable;

        #region Bind statements

        public virtual BoundStatement BindStatement(StmtSyntax stmt) => BindStatementCore(stmt).WithSyntax(stmt);

        BoundStatement BindStatementCore(StmtSyntax stmt)
        {
            Debug.Assert(stmt != null);

            if (stmt is ExpressionStmt exprStm)
                return new BoundExpressionStmt(BindExpression(exprStm.Expression, BoundAccess.None));
            if (stmt is ReturnStmt jmpStm)
                return BindReturnStmt(jmpStm);
            if (stmt is LocalDeclStmt varDecl)
                return BindVarDeclStmt(varDecl);

            Diagnostics.Add(GetLocation(stmt), ErrorCode.ERR_NotYetImplemented,
                $"Statement of type '{stmt.GetType().Name}'");
            return new BoundEmptyStmt(stmt.Span);
        }

        private BoundStatement BindVarDeclStmt(LocalDeclStmt varDecl)
        {
            var decl1 = varDecl.Declaration;

            foreach (var decl in decl1.Variables)
            {
                var localVar = Method.LocalsTable.BindLocalVariable(new VariableName(decl.Identifier.Text), decl);
                var boundExpression = BindExpression(decl.Initializer.Value);

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

        public TypeSymbol BindType(TypeEx tref)
        {
            var trf = DeclaringCompilation.CoreTypes;

            if (tref is PredefinedTypeEx pt)
            {
                switch (pt.Keyword.Kind())
                {
                    case SyntaxKind.IntKeyword: return trf.Int32;
                    case SyntaxKind.LongKeyword: return trf.Int64;
                    case SyntaxKind.VoidKeyword: return trf.Void;
                    case SyntaxKind.ObjectKeyword: return trf.Object;
                    case SyntaxKind.StringKeyword: return trf.String;
                    case SyntaxKind.BoolKeyword: return trf.Boolean;
                    case SyntaxKind.DatetimeKeyword: return trf.DateTime;

                    default: throw ExceptionUtilities.UnexpectedValue(pt.Kind());
                }
            }
            else if (tref is NameEx named)
            {
                var typeSymbol = FindTypeByName(named);
                return (TypeSymbol)typeSymbol;
            }
            else if (tref is UnionTypeEx union)
            {
                var listSym = new List<TypeSymbol>();
                foreach (var type in union.Types)
                {
                    var symbol = BindType(type);
                    listSym.Add(symbol);
                }

                //TODO: create Union type symbol
                var tsym = DeclaringCompilation.PlatformSymbolCollection.SynthesizeUnionType(
                    (NamespaceSymbol)DeclaringCompilation.GlobalNamespace, listSym);

                return tsym;
            }
            else
            {
                Diagnostics.Add(GetLocation(tref), ErrorCode.ERR_NotYetImplemented,
                    $"Expression of type '{tref.GetType().Name}'");

                throw new NotImplementedException();
            }
        }

        protected virtual ITypeSymbol FindTypeByName(NameEx tref)
        {
            return _next?.FindTypeByName(tref);
        }

        protected virtual IEnumerable<IMethodSymbol> FindMethodsByName(string name)
        {
            return _next?.FindMethodsByName(name);
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

        public BoundStatement BindEmptyStmt(Microsoft.CodeAnalysis.Text.TextSpan span)
        {
            return new BoundEmptyStmt(span);
        }

        #endregion

        #region Bind expressions

        public virtual BoundExpression BindExpression(ExprSyntax expr, BoundAccess access) =>
            BindExpressionCore(expr, access).WithSyntax(expr);

        protected BoundExpression BindExpression(ExprSyntax expr) => BindExpression(expr, BoundAccess.Read);

        protected BoundExpression BindExpressionCore(ExprSyntax expr, BoundAccess access)
        {
            Debug.Assert(expr != null);

            if (expr is LiteralEx) return BindLiteralEx((LiteralEx)expr).WithAccess(access);
            if (expr is NameEx ne) return BindNameEx(ne, access);

            if (expr is BinaryEx bex) return BindBinaryEx(bex).WithAccess(access);
            if (expr is AssignEx aex) return BindAssignEx(aex, access);
            // if (expr is PrefixUnaryEx pfue) return BindUnaryEx(ue, access).WithAccess(access);
            // if (expr is PostfixUnaryEx ptue) return BindUnaryEx(ue, access).WithAccess(access);
            // if (expr is IncDecEx incDec) return BindIncDec(incDec).WithAccess(access);
            if (expr is InvocationEx ce) return BindCallEx(ce).WithAccess(access);
            if (expr is MatchEx me) return BindMatchEx(me).WithAccess(access);
            if (expr is MemberAccessEx mae)
                return BindMemberAccessEx(mae, SyntaxFactory.ArgumentList(), false).WithAccess(access);
            if (expr is ElementAccessEx eae) return BindIndexerEx(eae).WithAccess(access);

            if (expr is ThrowEx throwEx)
                return new BoundThrowEx(BindExpression(throwEx.Expression, BoundAccess.Read), null);

            //
            Diagnostics.Add(GetLocation(expr), ErrorCode.ERR_NotYetImplemented,
                $"Expression of type '{expr.GetType().Name}'");
            return new BoundLiteral(null, null);
        }

        private BoundExpression BindMatchEx(MatchEx me)
        {
            var boundExpression = BindExpression(me.Expression);
            TypeSymbol resultType = null;
            var arms = new List<BoundMatchArm>();

            bool takeTypeFromNext = false;

            foreach (var arm in me.Arms)
            {
                var boundArm = BindMatchArm(arm);

                arms.Add(boundArm);

                var armType = (TypeSymbol)(boundArm.ResultType);

                if (armType != null)
                {
                    // then we try to calc result type

                    if (resultType != null)
                    {
                        if (resultType != armType)
                        {
                            if (resultType.IsEqualToOrDerivedFrom(armType))
                            {
                                //error
                            }

                            if (armType.IsEqualToOrDerivedFrom(resultType))
                            {
                                //cast
                            }
                        }
                    }
                    else
                    {
                        resultType = armType;
                    }
                }
            }

            if (resultType == null)
            {
                //error
            }

            return new BoundMatchEx(boundExpression, arms, resultType);
        }

        private BoundMatchArm BindMatchArm(MatchArm arm)
        {
            var pattern = BindExpression(arm.PatternExpression);

            var result = (arm.ResultExpression != null) ? BindExpression(arm.ResultExpression) : null;

            return new BoundMatchArm(pattern, null, result, result?.ResultType);
        }

        private BoundExpression BindIndexerEx(ElementAccessEx ie)
        {
            var array = BindExpression(ie.Expression);

            //TODO: make arity for element accsessex
            var indexer = BindExpression(ie.ArgumentList.Arguments.First().Expression);


            var accessIndexMethod = array.Type.GetMembers("get_Item").OfType<MethodSymbol>()
                .FirstOrDefault(x => x.ParameterCount == 1 && x.Parameters[0].Type == indexer.Type);

            if (accessIndexMethod != null)
                return new BoundArrayItemEx(DeclaringCompilation, array, indexer, accessIndexMethod.ReturnType);


            throw new NotImplementedException();
        }


        private BoundLiteral BindLiteralEx(LiteralEx node)
        {
            // bug.Assert(node.Kind == SyntaxKind.LiteralExpression);

            var value = node.Token.Value;

            ConstantValue cv;
            TypeSymbol type = null;

            if (value == null)
            {
                cv = ConstantValue.Null;
            }
            else
            {
                Debug.Assert(!value.GetType().GetTypeInfo().IsEnum);

                var specialType = SpecialTypeExtensions.FromRuntimeTypeOfLiteralValue(value);

                // C# literals can't be of type byte, sbyte, short, ushort:
                Debug.Assert(
                    specialType != SpecialType.None &&
                    specialType != SpecialType.System_Byte &&
                    specialType != SpecialType.System_SByte &&
                    specialType != SpecialType.System_Int16 &&
                    specialType != SpecialType.System_UInt16);

                cv = ConstantValue.Create(value, specialType);
                type = DeclaringCompilation.GetSpecialType(specialType);
            }

            return new BoundLiteral(cv.Value, type);
        }

        // protected BoundExpression BindLiteral(LiteralEx expr)
        // {
        //     var op = ToOp(expr.Kind());
        //
        //     switch (op)
        //     {
        //         case Operations.IntLiteral:
        //         case Operations.StringLiteral:
        //         case Operations.DoubleLiteral:
        //         case Operations.NullLiteral:
        //         case Operations.BinaryStringLiteral:
        //         case Operations.BoolLiteral:
        //             return new BoundLiteral(expr.ObjectiveValue, GetSymbolFromLiteralOperation(op));
        //
        //         case Operations.QueryLiteral:
        //
        //             var query = (string)expr.ObjectiveValue;
        //             var element = QLang.Parse(query, DeclaringCompilation.MetadataCollection);
        //
        //             var vars = element.Find<QVar>().ToArray();
        //
        //             foreach (var @var in vars)
        //             {
        //                 var bVar = BindVariableName(@var.Name, null);
        //                 var vType = bVar.Type;
        //                 var smType = MetadataTypeProvider.Resolve(DeclaringCompilation, vType);
        //
        //                 @var.BindType(smType);
        //             }
        //
        //             //var transforms into parameter
        //
        //
        //             //TODO: Add errors to the DiagnosticBug if we catch error
        //             //DeclaringCompilation.GetDiagnostics()
        //
        //             return new BoundLiteral(expr.ObjectiveValue, GetSymbolFromLiteralOperation(expr.Operation));
        //
        //         default:
        //             throw new NotImplementedException();
        //     }
        // }

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

        protected BoundExpression BindNameEx(NameEx expr, BoundAccess access)
        {
            //handle special wildcard symbol
            var identifier = expr.GetUnqualifiedName().Identifier.Text;

            if (identifier == "_")
            {
                return new BoundWildcardEx(DeclaringCompilation.CoreTypes.Void.Symbol).WithSyntax(expr);
            }

            var bounded = BindSimpleVarUse(expr, access);

            // local variable hide the members
            if (bounded != null)
                return bounded;


            //try to find property
            //TODO: add scan fields 
            var result = Container.GetMembers(identifier).OfType<PropertySymbol>().FirstOrDefault();
            if (result != null)
            {
                var type = Container as ITypeSymbol;
                var th = new BoundVariableRef("this", type);

                return new BoundPropertyRef(result, th).WithAccess(access);
            }

            //try to find type name
            var foundedType = BindType(expr);

            return new BoundClassTypeRef(QualifiedName.Object, Method, foundedType);
        }

        protected BoundExpression BindSimpleVarUse(NameEx expr, BoundAccess access)
        {
            var varname = BindVariableName(expr);

            // variable not found
            if (varname is null) return null;


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

        protected BoundVariableName BindVariableName(NameEx nameEx)
        {
            Debug.Assert(nameEx != null);
            return BindVariableName(nameEx.GetUnqualifiedName().Identifier.Text, nameEx);
        }

        protected BoundVariableName BindVariableName(string varName, AquilaSyntaxNode syntax)
        {
            Debug.Assert(varName != null);
            Debug.Assert(Method != null);

            Method.LocalsTable.TryGetVariable(new VariableName(varName), out var localVar);
            if (localVar is null)
                return null;

            var type = localVar.Type;
            return new BoundVariableName(varName, type).WithSyntax(syntax);
        }

        protected Location GetLocation(AquilaSyntaxNode expr) => expr.SyntaxTree.GetLocation(expr);

        protected virtual BoundExpression BindBinaryEx(BinaryEx expr)
        {
            BoundAccess laccess = BoundAccess.Read;


            var op = expr.OperatorToken.Kind() switch
            {
                SyntaxKind.PlusToken => Operations.Add,
                SyntaxKind.AsteriskToken => Operations.Mul,
                SyntaxKind.SlashToken => Operations.Div,
                SyntaxKind.MinusToken => Operations.Sub,
                SyntaxKind.ExclamationEqualsToken => Operations.NotEqual,
                _ => Operations.Unknown
            };

            var left = BindExpression(expr.Left, laccess);
            var right = BindExpression(expr.Right, BoundAccess.Read);


            return new BoundBinaryEx(left, right, op, left.ResultType);
        }

        protected BoundExpression BindUnaryEx(PrefixUnaryEx expr, BoundAccess access)
        {
            var operandAccess = BoundAccess.Read;

            switch (expr.Kind())
            {
                // case SyntaxKind.:
                //     operandAccess = access; // TODO: | Quiet
                //     break;
                // case Operations.UnsetCast:
                //     operandAccess = BoundAccess.None;
                //     break;
            }

            var boundOperation = BindExpression(expr.Operand, operandAccess);

            switch (expr.Kind())
            {
                case SyntaxKind.BoolKeyword:
                    return new BoundConversionEx(boundOperation, PrimitiveBoundTypeRefs.BoolTypeRef, null);

                case SyntaxKind.IntKeyword:
                case SyntaxKind.LongKeyword:
                    return new BoundConversionEx(boundOperation, PrimitiveBoundTypeRefs.LongTypeRef, null);

                case SyntaxKind.DecimalKeyword:
                case SyntaxKind.DoubleKeyword:
                case SyntaxKind.FloatKeyword:
                    return new BoundConversionEx(boundOperation, PrimitiveBoundTypeRefs.DoubleTypeRef, null);

                // case Operations.UnicodeCast:
                //     return new BoundConversionEx(boundOperation, PrimitiveBoundTypeRefs.StringTypeRef, null);

                case SyntaxKind.StringKeyword:

                    // // workaround (binary) is parsed as (StringCast)
                    // if (expr.ContainingSourceUnit.GetSourceCode(expr.Span).StartsWith("(binary)"))
                    // {
                    //     goto case Operations.BinaryCast;
                    // }

                    return
                        new BoundConversionEx(boundOperation,
                            PrimitiveBoundTypeRefs.StringTypeRef, null);

                default:
                    return new BoundUnaryEx(BindExpression(expr.Operand, operandAccess), ToOp(expr.Kind()), null);
            }
        }


        private static Operations ToOp(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.SimpleAssignmentExpression:
                    return Operations.AssignValue;
                default:
                    return Operations.Unknown;
            }
        }

        protected BoundExpression BindAssignEx(AssignEx expr, BoundAccess access)
        {
            var boundExpr = BindExpression(expr.Left, BoundAccess.Write);


            if (!(boundExpr is BoundReferenceEx target))
            {
                throw new Exception("Can't assign values to the not reference store");
                return null;
            }

            BoundExpression value;

            value = BindExpression(expr.Right, BoundAccess.Read);

            // if (expr.Operation == Operations.AssignValue && !(target is BoundListEx))
            // {
            //     value = BindCopyValue(value);
            // }


            var op = ToOp(expr.Kind());

            if (op == Operations.AssignValue || op == Operations.AssignRef)
            {
                return new BoundAssignEx(target, value, value.ResultType).WithAccess(access);
            }
            else
            {
                if (target is BoundArrayItemEx itemex && itemex.Index == null)
                {
                    // Special case:
                    switch (op)
                    {
                        case Operations.AssignPrepend:
                        case Operations.AssignAppend:
                            break;
                        default:
                            value = new BoundBinaryEx(new BoundLiteral(null, null).WithAccess(BoundAccess.Read), value,
                                AstUtils.CompoundOpToBinaryOp(op), null);
                            break;
                    }

                    return new BoundAssignEx(target, value.WithAccess(BoundAccess.Read), value.ResultType)
                        .WithAccess(access);
                }
                else
                {
                    target.Access = target.Access.WithRead();

                    return new BoundCompoundAssignEx(target, value, op, value.ResultType)
                        .WithAccess(access);
                }
            }
        }

        // protected BoundExpression BindIncDec(IncDecEx expr)
        // {
        //     // bind variable reference
        //     var varref = (BoundReferenceEx)BindExpression(expr.Operand, BoundAccess.ReadAndWrite);
        //
        //     //
        //     return new BoundIncDecEx(varref, expr.IsIncrement, expr.IsPost, null);
        // }

        private BoundExpression BindCallEx(InvocationEx expr)
        {
            return BindMethodGroup(expr.Expression, expr.ArgumentList, true);
        }

        private BoundExpression BindMethodGroup(ExprSyntax expr, ArgumentListSyntax args, bool invoked)
        {
            switch (expr.Kind())
            {
                case SyntaxKind.IdentifierEx:
                    return BindName((SimpleNameEx)expr, args, invoked);
                case SyntaxKind.SimpleMemberAccessExpression:
                    return BindMemberAccessEx((MemberAccessEx)expr, args, invoked);
                case SyntaxKind.GenericName:
                    return BindName((GenericEx)expr, args, invoked);

                default:
                    throw new NotImplementedException();
            }
        }

        // helper
        ImmutableArray<IMethodSymbol> Construct(ImmutableArray<IMethodSymbol> methods,
            ImmutableArray<ITypeSymbol> typeArgs)
        {
            if (typeArgs.IsDefaultOrEmpty)
            {
                return methods;
            }
            else
            {
                var result = new List<IMethodSymbol>();

                for (int i = 0; i < methods.Length; i++)
                {
                    if (methods[i].Arity == typeArgs.Length) // TODO: check the type argument is assignable
                    {
                        result.Add(methods[i].Construct(typeArgs.ToArray()));
                    }
                }

                return result.ToImmutableArray();
            }
        }

        protected virtual BoundExpression BindName(SimpleNameEx expr, ArgumentListSyntax argumentList, bool invocation)
        {
            Debug.Assert(Method != null);

            var arglist = argumentList.Arguments
                .Select(x => BoundArgument.Create(BindExpression(x.Expression)))
                .ToImmutableArray();

            var typeArgs = ImmutableArray<ITypeSymbol>.Empty;

            if (expr.Kind() == SyntaxKind.GenericName)
            {
                typeArgs = ((GenericEx)expr).TypeArgumentList.Arguments.Select(x => BindType(x) as ITypeSymbol)
                    .ToImmutableArray();
            }


            var containerMembers = Container.GetMembers(expr.Identifier.Text).Where(x => x is MethodSymbol)
                .ToList();

            if (containerMembers.Count() == 1)
            {
                var member = containerMembers[0];
                if (invocation)
                {
                    if (member is MethodSymbol ms)
                    {
                        if (ms.IsStatic)
                        {
                            return new BoundStaticCallEx(ms,
                                new BoundMethodName(new QualifiedName(new Name(expr.Identifier.Text))),
                                arglist, ImmutableArray<ITypeSymbol>.Empty, null);
                        }
                        else
                        {
                            var type = Container as ITypeSymbol;

                            var th = new BoundVariableRef("this", type);

                            return new BoundInstanceCallEx(ms,
                                new BoundMethodName(new QualifiedName(new Name(expr.Identifier.Text))),
                                ImmutableArray<BoundArgument>.Empty, ImmutableArray<ITypeSymbol>.Empty, th, null
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

            if (containerMembers.Count() > 1 && invocation)
            {
                //Resolve overload
                throw new NotImplementedException("Overload not supported for now");
            }

            if (!Enumerable.Any(containerMembers))
                Diagnostics.Add(GetLocation(expr), ErrorCode.INF_CantResolveSymbol);

            //TODO: Add global built-in methods and try to resolve it here, if local members not found
            var globalMembers = FindMethodsByName(expr.Identifier.Text)
                .Where(x => x.Arity == typeArgs.Count()).ToImmutableArray();

            globalMembers = Construct(globalMembers, typeArgs);

            if (globalMembers.Length == 1)
            {
                var member = globalMembers[0];

                if (invocation)
                {
                    if (member is MethodSymbol ms)
                    {
                        if (ms.IsStatic)
                        {
                            return new BoundStaticCallEx(ms,
                                new BoundMethodName(new QualifiedName(new Name(expr.Identifier.Text))),
                                arglist, typeArgs, ms.ReturnType);
                        }
                        else
                        {
                            //Extension method can't be instance
                        }
                    }
                }
            }

            throw new Exception("Test");
        }

        private BoundExpression BindMemberAccessEx(MemberAccessEx expr, ArgumentListSyntax args, bool invoke)
        {
            var boundLeft = BindExpression(expr.Expression).WithAccess(BoundAccess.Invoke);

            var leftType = boundLeft.Type;

            var members = leftType.GetMembers(expr.Name.GetUnqualifiedName().Identifier.Text);
            foreach (var member in members)
            {
                if (invoke)
                {
                    if (member is MethodSymbol ms)
                    {
                        var arglist = args.Arguments.Select(x => BoundArgument.Create(BindExpression(x.Expression)))
                            .ToImmutableArray();


                        if (ms.IsStatic)
                            return new BoundStaticCallEx(ms,
                                new BoundMethodName(QualifiedName.Parse(expr.Name.GetUnqualifiedName().Identifier.Text,
                                    true)),
                                arglist,
                                ImmutableArray<ITypeSymbol>.Empty, ms.ReturnType);

                        else

                            return new BoundInstanceCallEx(ms,
                                new BoundMethodName(QualifiedName.Parse(expr.Name.GetUnqualifiedName().Identifier.Text,
                                    true)),
                                arglist, ImmutableArray<ITypeSymbol>.Empty,
                                boundLeft, ms.ReturnType);
                    }
                }
                else
                {
                    if (member is PropertySymbol ps)
                    {
                        return new BoundPropertyRef(ps, boundLeft);
                    }
                }
            }

            Diagnostics.Add(GetLocation(expr.Name), ErrorCode.ERR_MethodNotFound,
                expr.Name.GetUnqualifiedName().Identifier.Text, leftType.Name);

            return new BoundLiteral(0, DeclaringCompilation.CoreTypes.Int32.Symbol);
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
}