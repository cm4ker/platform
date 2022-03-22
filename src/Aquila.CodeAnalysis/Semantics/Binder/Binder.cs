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
using Aquila.Core.Querying;
using Aquila.Core.Querying.Model;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Declarations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.PooledObjects;
using Xunit;
using EnumerableExtensions = Roslyn.Utilities.EnumerableExtensions;
using SpecialTypeExtensions = Aquila.CodeAnalysis.Symbols.SpecialTypeExtensions;

namespace Aquila.CodeAnalysis.Semantics
{
    internal class InClrImportBinder : Binder
    {
        private readonly INamespaceOrTypeSymbol _container;

        public InClrImportBinder(INamespaceOrTypeSymbol container, Binder next) : base(next)
        {
            _container = container;
        }

        protected override ITypeSymbol FindTypeByName(NameEx tref)
        {
            var qName = tref.GetUnqualifiedName().Identifier.Text;

            var typeMembers = Container.GetTypeMembers(qName, -1);

            if (typeMembers.Length == 1)
                return typeMembers[0];

            if (typeMembers.Length == 0)
            {
                return Next.BindType(tref);
            }

            if (typeMembers.Length > 1)
            {
                Diagnostics.Add(GetLocation(tref), ErrorCode.WRN_UndefinedType,
                    $"Expression of type '{tref.GetType().Name}'");
            }

            return null;
        }

        protected override void FindMethodsByName(string name, ArrayBuilder<Symbol> result)
        {
            var typesCandidate = _container.GetTypeMembers().Where(x => name.StartsWith(x.Name.ToSnakeCase()));

            foreach (var type in typesCandidate)
            {
                var typeSnake = type.Name.ToSnakeCase();

                var resolvedMethods = type.GetMembers().Where(x =>
                        x.DeclaredAccessibility == Accessibility.Public && x.IsStatic &&
                        typeSnake + "_" + x.Name.ToSnakeCase() == name)
                    .OfType<MethodSymbol>();

                result.AddRange(resolvedMethods);
            }

            base.FindMethodsByName(name, result);
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
            TypeSymbol result = null;

            var qName = tref.GetUnqualifiedName().Identifier.Text;

            var typeMembers = Container.GetTypeMembers(qName, -1);

            if (typeMembers.Length == 1)
                result = typeMembers[0];

            if (typeMembers.Length == 0)
            {
                result = Next?.BindType(tref);
            }

            if (typeMembers.Length > 1)
            {
                Diagnostics.Add(GetLocation(tref), ErrorCode.WRN_UndefinedType,
                    $"Expression of type '{tref.GetType().Name}'");
            }

            if (result == null)
            {
                result = new MissingMetadataTypeSymbol(qName, 0, true);
            }

            if (result.IsErrorType())
                Diagnostics.Add(GetLocation(tref), ErrorCode.ERR_TypeNameCannotBeResolved, qName);

            return result;
        }
    }

    internal class InModuleBinder : Binder
    {
        public InModuleBinder(Binder next) : base(next)
        {
        }
    }

    internal class InMethodBinder : Binder
    {
        private readonly SourceMethodSymbol _method;
        private LocalsTable _locals;

        public InMethodBinder(SourceMethodSymbol method, Binder next) : base(next)
        {
            _method = method ?? throw new ArgumentNullException(nameof(method));
        }

        public override SourceMethodSymbol Method => _method;


        public override NamespaceOrTypeSymbol Container => _method.ContainingType;
    }

    internal class GlobalBinder : InContainerBinder
    {
        private readonly NamespaceOrTypeSymbol _ns;

        public GlobalBinder(INamespaceOrTypeSymbol ns, Binder next) : base(ns, next)
        {
            _ns = (NamespaceOrTypeSymbol)ns;
        }

        public override NamespaceOrTypeSymbol Container => _ns;

        protected override void FindMethodsByName(string name, ArrayBuilder<Symbol> result)
        {
            result.AddRange(_ns.GetMembers(name).OfType<MethodSymbol>());
        }
    }

    abstract class Binder
    {
        internal Binder(AquilaCompilation compilation)
        {
            Compilation = compilation;
        }


        public Binder(Binder next)
        {
            Next = next;
            Compilation = next.Compilation;
        }

        public Binder Next { get; }

        public virtual Binder GetBinder(AquilaSyntaxNode syntax)
        {
            return Next.GetBinder(syntax);
        }

        public DiagnosticBag Diagnostics => Compilation.DeclarationDiagnostics;

        /// <summary>Gets <see cref="PrimitiveBoundTypeRefs"/> instance.</summary>
        internal PrimitiveBoundTypeRefs PrimitiveBoundTypeRefs => Compilation.TypeRefs;

        public AquilaCompilation Compilation { get; }

        public virtual SourceMethodSymbol Method { get; }

        public virtual NamespaceOrTypeSymbol Container { get; }

        public LocalsTable Locals => Method.LocalsTable;

        public virtual ImmutableArray<TypeSymbol> DeclaredTypes => ImmutableArray<TypeSymbol>.Empty;

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
                if (string.IsNullOrWhiteSpace(decl.Identifier.Text) || decl.Initializer == null)
                {
                    Diagnostics.Add(GetLocation(varDecl), ErrorCode.ERR_MissingIdentifierSymbol);
                    return new BoundExpressionStmt(
                        new BoundBadEx(this.Compilation.GetSpecialType(SpecialType.System_Void)));
                }

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
            var trf = Compilation.CoreTypes;

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
                var tsym = Compilation.PlatformSymbolCollection.SynthesizeUnionType(
                    (NamespaceSymbol)Compilation.SourceModule.GlobalNamespace, listSym);

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
            return Next?.FindTypeByName(tref);
        }

        protected virtual void FindMethodsByName(string name, ArrayBuilder<Symbol> result)
        {
            Next?.FindMethodsByName(name, result);
        }

        protected BoundStatement BindReturnStmt(ReturnStmt stmt)
        {
            Debug.Assert(Method != null);

            BoundExpression expr = null;

            if (stmt.Expression != null)
            {
                expr = BindExpression(stmt.Expression, BoundAccess.Read);
            }

            return new BoundReturnStmt(expr);
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
            if (expr is InvocationEx ce) return BindCallEx(ce).WithAccess(access);
            if (expr is MatchEx me) return BindMatchEx(me).WithAccess(access);
            if (expr is MemberAccessEx mae)
                return BindMemberAccessEx(mae, SyntaxFactory.ArgumentList(), false).WithAccess(access);
            if (expr is ElementAccessEx eae) return BindIndexerEx(eae).WithAccess(access);
            if (expr is PostfixUnaryEx pue) return BindPostfixUnaryEx(pue).WithAccess(access);
            if (expr is PrefixUnaryEx prue) return BindPrefixUnaryEx(prue).WithAccess(access);
            if (expr is ThrowEx throwEx)
                return new BoundThrowEx(BindExpression(throwEx.Expression, BoundAccess.Read), null);
            if (expr is AllocEx allocEx) return BindAllocEx(allocEx).WithAccess(access);
            if (expr is ParenthesizedEx pe) return BindExpression(pe.Expression).WithAccess(access);

            Diagnostics.Add(GetLocation(expr), ErrorCode.ERR_NotYetImplemented,
                $"Expression of type '{expr.GetType().Name}'");

            return new BoundBadEx(this.Compilation.GetSpecialType(SpecialType.System_Void));
        }

        private BoundExpression BindAllocEx(AllocEx ex)
        {
            var type = (NamedTypeSymbol)BindType(ex.Name);
            var inits = new List<BoundAllocExAssign>();

            if (ex.Initializer.Expressions.Any())
                switch (ex.Initializer.Kind())
                {
                    case SyntaxKind.ObjectInitializerExpression:
                        foreach (var iex in ex.Initializer.Expressions)
                        {
                            var ae = iex as AssignEx;

                            var left = (ae.Left as IdentifierEx);
                            var right = BindExpression(ae.Right);

                            var mems = type.GetMembers(left.Identifier.Text)
                                .Where(x => !x.IsStatic && ((x.Kind & SymbolKind.Property) > 0
                                                            || (x.Kind & SymbolKind.Field) > 0))
                                .ToImmutableArray();
                            if (mems.Length > 1)
                            {
                                //ambiguity
                            }
                            else if (mems.Length == 1)
                            {
                                var item = mems[0];

                                inits.Add(new BoundAllocExAssign(item, right));
                            }
                        }

                        break;

                    default:
                        throw new NotImplementedException();
                }

            return new BoundAllocEx(type, inits, type);

            // //we need parameterless constructor 
            // var ctor = type.InstanceConstructors.FirstOrDefault(x => x.ParameterCount == 0);
            //
            // if (ctor == null)
            //     throw new Exception("Parameterless constructor not found");
            //
            // var boundNewEx = new BoundNewEx(ctor, type, ImmutableArray<BoundArgument>.Empty,
            //     ImmutableArray<ITypeSymbol>.Empty, type);
            //
            // if (!ex.Initializer.Expressions.Any())
            // {
            //     //we just create new type
            //     return boundNewEx;
            // }
            //
            // var instance = new BoundTemporalVariableRef(new BoundVariableName(NextTmpVariableName(), type), type);
            // var tmpVar = Locals.BindTemporalVariable(instance.Name.NameValue, type);
            //
            // var firstAssign = new BoundAssignEx(instance, boundNewEx.WithAccess(BoundAccess.Read));
            //
            // var kind = ex.Initializer.Kind();
            //
            // switch (kind)
            // {
            //     case SyntaxKind.ObjectInitializerExpression:
            //         var list = new List<BoundAssignEx>();
            //         list.Add(firstAssign);
            //
            //         foreach (var iex in ex.Initializer.Expressions)
            //         {
            //             var ae = iex as AssignEx;
            //
            //             var left = (ae.Left as IdentifierEx);
            //             var right = BindExpression(ae.Right);
            //
            //             var mems = type.GetMembers(left.Identifier.Text)
            //                 .Where(x => !x.IsStatic && (x.Kind & (SymbolKind.Field | SymbolKind.Property)) > 0)
            //                 .ToImmutableArray();
            //             if (mems.Length > 1)
            //             {
            //                 //ambiguity
            //             }
            //             else if (mems.Length == 1)
            //             {
            //                 var item = mems[0];
            //
            //                 BoundReferenceEx target = item.Kind switch
            //                 {
            //                     SymbolKind.Field => new BoundFieldRef((IFieldSymbol)item, instance),
            //                     SymbolKind.Property => new BoundPropertyRef((IPropertySymbol)item, instance),
            //                     _ => throw new NotImplementedException("this symbol kind is not implemented")
            //                 };
            //
            //                 list.Add(new BoundAssignEx(target.WithAccess(BoundAccess.Write),
            //                     right.WithAccess(BoundAccess.Read)));
            //             }
            //         }
            //
            //         return new BoundMultiAssignEx(instance, list);
            //
            //         break;
            //     case SyntaxKind.CollectionInitializerExpression:
            //     case SyntaxKind.ArrayInitializerExpression:
            //         throw new NotImplementedException();
            //         break;
            // }
            //
            // return null;
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
                return new BoundArrayItemEx(Compilation, array, indexer, accessIndexMethod.ReturnType);


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
                type = Compilation.GetSpecialType(specialType);
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
            var ct = Compilation.CoreTypes;

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

        protected virtual BoundExpression BindNameEx(NameEx expr, BoundAccess access)
        {
            //handle special wildcard symbol
            var identifier = expr.GetUnqualifiedName().Identifier.Text;

            if (identifier == "_")
            {
                return new BoundWildcardEx(Compilation.CoreTypes.Void.Symbol).WithSyntax(expr);
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
                        //TODO: Add diagnostics
                        throw new NotImplementedException();
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


            var op = expr.Kind() switch
            {
                SyntaxKind.AddExpression => Operations.Add,
                SyntaxKind.MultiplyExpression => Operations.Mul,
                SyntaxKind.DivideExpression => Operations.Div,
                SyntaxKind.SubtractExpression => Operations.Sub,
                SyntaxKind.NotEqualsExpression => Operations.NotEqual,
                SyntaxKind.EqualsExpression => Operations.Equal,
                SyntaxKind.BitwiseOrExpression => Operations.BitOr,
                SyntaxKind.BitwiseAndExpression => Operations.BitAnd,
                SyntaxKind.ExclusiveOrExpression => Operations.BitXor,
                SyntaxKind.BitwiseNotExpression => Operations.BitNegation,
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
                Diagnostics.Add(GetLocation(expr), ErrorCode.ERR_TypeNameCannotBeResolved,
                    $"Can't assign to not reference expression");

                return new BoundBadEx(boundExpr.Type).WithSyntax(expr);
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

        protected BoundExpression BindPostfixUnaryEx(PostfixUnaryEx expr)
        {
            // bind variable reference
            var varref = (BoundReferenceEx)BindExpression(expr.Operand, BoundAccess.ReadAndWrite);


            switch (expr.Kind())
            {
                case SyntaxKind.PostIncrementExpression:
                    return new BoundIncDecEx(varref, isIncrement: true, true, varref.ResultType);
                case SyntaxKind.PostDecrementExpression:
                    return new BoundIncDecEx(varref, isIncrement: false, true, varref.ResultType);

                default: throw new NotSupportedException();
            }
        }

        protected BoundExpression BindPrefixUnaryEx(PrefixUnaryEx expr)
        {
            // bind variable reference
            var bound = BindExpression(expr.Operand, BoundAccess.ReadAndWrite);


            switch (expr.Kind())
            {
                case SyntaxKind.PreIncrementExpression when bound is BoundReferenceEx br:
                    return new BoundIncDecEx(br, isIncrement: true, isPostfix: false, bound.ResultType);
                case SyntaxKind.PreDecrementExpression when bound is BoundReferenceEx br:
                    return new BoundIncDecEx(br, isIncrement: false, isPostfix: false, bound.ResultType);
                case SyntaxKind.UnaryMinusExpression:
                    return new BoundUnaryEx(bound, Operations.Minus, bound.ResultType);
                case SyntaxKind.BitwiseNotExpression:
                    return new BoundUnaryEx(bound, Operations.BitNegation, bound.ResultType);
                default: throw new NotSupportedException();
            }
        }

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

        MethodSymbol ConstructSingle(MethodSymbol method, ImmutableArray<ITypeSymbol> typeArgs)
        {
            if (typeArgs.IsDefaultOrEmpty)
            {
                return method;
            }

            if (method.Arity == typeArgs.Length)
            {
                return method.Construct(typeArgs.ToArray());
            }

            throw new InvalidOperationException("TODO: generic types different arity");
        }

        // helper
        ImmutableArray<MethodSymbol> Construct(ImmutableArray<MethodSymbol> methods,
            ImmutableArray<ITypeSymbol> typeArgs)
        {
            if (typeArgs.IsDefaultOrEmpty)
            {
                return methods;
            }
            else
            {
                var result = new List<MethodSymbol>();

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
                typeArgs = ((GenericEx)expr).TypeArgumentList.Arguments.Select(BindType).OfType<ITypeSymbol>()
                    .ToImmutableArray();
            }

            var nameId = expr.Identifier.Text;

            //try find local first
            var containerMembers = Container.GetMembers(nameId).ToImmutableArray();

            var qualifiedName = new QualifiedName(new Name(nameId));

            if (containerMembers.Count() == 1)
            {
                var member = containerMembers[0];
                if (invocation)
                {
                    if (member is MethodSymbol ms)
                    {
                        ms = ConstructSingle(ms, typeArgs);

                        if (ms.IsStatic)
                        {
                            return new BoundStaticCallEx(ms,
                                new BoundMethodName(qualifiedName), arglist, typeArgs, ms.ReturnType);
                        }
                        else
                        {
                            var type = Container as ITypeSymbol;

                            var th = new BoundVariableRef(QualifiedName.This.Name.Value, type);
                            return new BoundInstanceCallEx(ms, new BoundMethodName(qualifiedName),
                                arglist, typeArgs, th, ms.ReturnType);
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
                var overload = ResolveOverload(containerMembers.OfType<MethodSymbol>().ToImmutableArray(), arglist);
            }

            //Go to global members
            {
                var methods = new ArrayBuilder<Symbol>();
                FindMethodsByName(nameId, methods);

                var globalMembers = methods.OfType<MethodSymbol>().Where(x => x.Arity == typeArgs.Count())
                    .ToImmutableArray();

                globalMembers = Construct(globalMembers, typeArgs);

                var overload = ResolveOverload(globalMembers, arglist);

                if (overload == null)
                {
                    Diagnostics.Add(GetLocation(expr), ErrorCode.ERR_TypeNameCannotBeResolved, nameId);

                    //error can't resolve overload for this parameters
                    return new BoundBadEx(this.Compilation.GetSpecialType(SpecialType.System_Void));
                }
                else
                {
                    if (invocation)
                    {
                        if (overload is MethodSymbol ms)
                        {
                            if (ms.IsStatic)
                            {
                                return new BoundStaticCallEx(ms, new BoundMethodName(qualifiedName),
                                    arglist, typeArgs, ms.ReturnType);
                            }
                            else
                            {
                                //Extension method can't be instance
                            }
                        }
                    }
                }
            }

            if (!Enumerable.Any(containerMembers))
                Diagnostics.Add(GetLocation(expr), ErrorCode.INF_CantResolveSymbol);

            BoundExpression result;

            if (invocation)
            {
                result = new BoundStaticCallEx(new MissingMethodSymbol(nameId), new BoundMethodName(qualifiedName),
                    arglist, typeArgs, null);
            }
            else
            {
                result = new BoundPropertyRef(new MissingPropertySymbol(nameId), null);
            }

            return result;
        }

        private IMethodSymbol ResolveOverload(ImmutableArray<MethodSymbol> overloads,
            ImmutableArray<BoundArgument> argsList)
        {
            var types = argsList.Select(x => x.Type).ToImmutableArray();

            Dictionary<MethodSymbol, int> candidates = new();

            if (overloads.Length > 0)

                foreach (var methodSymbol in overloads)
                {
                    var cost = 0;

                    var parametars = methodSymbol.Parameters;

                    var paramPlace = 0;
                    if (methodSymbol.HasParamPlatformContext)
                    {
                        paramPlace = 1;
                    }

                    if (types.Length != parametars.Length - paramPlace)
                        continue;

                    var isCandidate = true;

                    for (int i = 0; i < types.Length; i++)
                    {
                        var leftType = types[i];
                        var rightType = parametars[i + paramPlace].Type;

                        if (!leftType.IsEqualOrDerivedFrom(rightType))
                        {
                            isCandidate = false;
                            break;
                        }
                        else
                        {
                            if (leftType.Equals(rightType))
                            {
                                cost += 1;
                            }
                            else
                            {
                                //need box type
                                cost += 2;
                            }
                        }
                    }

                    if (isCandidate)
                        candidates[methodSymbol] = cost;
                }

            if (candidates.Any())
                //minimize cost
                return candidates.MinBy(x => x.Value).Key;

            return null;
        }

        private BoundExpression BindMemberAccessEx(MemberAccessEx expr, ArgumentListSyntax args, bool invoke)
        {
            var boundLeft = BindExpression(expr.Expression).WithAccess(BoundAccess.Invoke);

            var leftType = boundLeft.Type;

            var identifierText = expr.Name.GetUnqualifiedName().Identifier.Text;
            var qualifiedName = QualifiedName.Parse(identifierText, true);

            //TODO: transform members names
            var members = leftType.GetMembers(identifierText);
            var typeArgs = ImmutableArray<ITypeSymbol>.Empty;

            if (expr.Name.Kind() == SyntaxKind.GenericName)
            {
                typeArgs = ((GenericEx)expr.Name).TypeArgumentList.Arguments.Select(BindType).OfType<ITypeSymbol>()
                    .ToImmutableArray();
            }

            foreach (var member in members)
            {
                if (invoke)
                {
                    if (member is MethodSymbol ms)
                    {
                        var arglist = args.Arguments.Select(x => BoundArgument.Create(BindExpression(x.Expression)))
                            .ToImmutableArray();

                        ms = ConstructSingle(ms, typeArgs);

                        if (ms.IsStatic)
                            return new BoundStaticCallEx(ms,
                                new BoundMethodName(qualifiedName), arglist, typeArgs, ms.ReturnType);

                        else

                            return new BoundInstanceCallEx(ms,
                                new BoundMethodName(qualifiedName), arglist, typeArgs, boundLeft, ms.ReturnType);
                    }
                }
                else
                {
                    if (member is PropertySymbol ps)
                    {
                        return new BoundPropertyRef(ps, boundLeft);
                    }
                    else if (member is FieldSymbol fs)
                        return new BoundFieldRef(fs, boundLeft);
                }
            }

            Diagnostics.Add(GetLocation(expr.Name), ErrorCode.ERR_MethodNotFound, identifierText, leftType.Name);

            return new BoundLiteral(0, Compilation.CoreTypes.Int32.Symbol);
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