using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Aquila.CodeAnalysis.Errors;
using Aquila.CodeAnalysis.FlowAnalysis;
using Aquila.CodeAnalysis.Semantics.TypeRef;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Aquila.CodeAnalysis.Syntax;
using Aquila.CodeAnalysis.Utilities;
using Aquila.Syntax.Ast;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;
using SpecialTypeExtensions = Aquila.CodeAnalysis.Symbols.SpecialTypeExtensions;

namespace Aquila.CodeAnalysis.Semantics
{
    internal abstract class Binder
    {
        private int _tmpVariableIndex = 0;

        private string NextTempVariableName() => "<tmp>'" + _tmpVariableIndex++;

        private string NextMatchVariableName() => "<match>'" + _tmpVariableIndex++;


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

        public virtual SourceMethodSymbolBase Method { get; }

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
            if (stmt is ForEachStmt frch)
                return BindForeachStmt(frch);
            if (stmt is TryStmt tryStmt)
                return BindTryStmt(tryStmt);

            Diagnostics.Add(GetLocation(stmt), ErrorCode.ERR_NotYetImplemented,
                $"Statement of type '{stmt.GetType().Name}'");
            return new BoundEmptyStmt(stmt.Span);
        }

        private BoundStatement BindTryStmt(TryStmt tryStmt)
        {
            foreach (var c in tryStmt.Catches)
            {
                if (c.Declaration != null)
                {
                    var type = BindType(c.Declaration.Type);
                    var v = BindVar(c.Declaration.Identifier, type);
                }
            }

            throw new NotImplementedException();
        }

        internal BoundStatement BindVarDecl(VariableDecl varDecl)
        {
            Debug.Assert(varDecl.Variables.Count == 1);

            var decl = varDecl.Variables.First();

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
        }

        private BoundStatement BindVarDeclStmt(LocalDeclStmt varDecl)
        {
            return BindVarDecl(varDecl.Declaration);
        }

        private BoundVariableRef BindVariable(VariableName varName, TextSpan span, TypeSymbol type)
        {
            var localVar = Method.LocalsTable.BindLocalVariable(varName, span, type);
            return BindVariable(localVar);
        }

        private BoundVariableRef BindVariable(IVariableReference localVar)
        {
            return new BoundVariableRef(new BoundVariableName(localVar.Symbol.Name, localVar.Type), localVar.Type);
        }

        private static BoundStatement BoundStatementError()
            => new BoundBadStmt();

        private BoundStatement BindForeachStmt(ForEachStmt stmt)
        {
            var collection = BindExpression(stmt.Expression, BoundAccess.Read);
            var colType = (TypeSymbol)collection.ResultType;

            var getEnumMethod = colType.LookupMember<MethodSymbol>(WellKnownMemberNames.GetEnumeratorMethodName);

            if (getEnumMethod == null)
                return BoundStatementError();

            var enumeratorSymbol = getEnumMethod.ReturnType;
            var currentMember = enumeratorSymbol
                .LookupMember<PropertySymbol>(WellKnownMemberNames.CurrentPropertyName);
            var moveNextMember =
                enumeratorSymbol.LookupMember<MethodSymbol>(WellKnownMemberNames.MoveNextMethodName,
                    m => !m.IsStatic && m.ParameterCount == 0);
            var disposeMember = enumeratorSymbol.LookupMember<MethodSymbol>(WellKnownMemberNames.DisposeMethodName,
                m => !m.IsStatic && m.ParameterCount == 0);

            if (currentMember == null || moveNextMember == null)
                return BoundStatementError();

            var variableType = currentMember.Type;

            var localVar = Method.LocalsTable.BindLocalVariable(new VariableName(stmt.Identifier.Text),
                stmt.Identifier.Span, variableType);

            var assign = CreateTmpAndAssign(enumeratorSymbol,
                new BoundCallEx(getEnumMethod, ImmutableArray<BoundArgument>.Empty,
                    ImmutableArray<ITypeSymbol>.Empty, collection, enumeratorSymbol
                ).WithAccess(BoundAccess.Read));

            //enumerator.MoveNext() expr
            var moveNextCondition = new BoundCallEx(moveNextMember,
                ImmutableArray<BoundArgument>.Empty,
                ImmutableArray<ITypeSymbol>.Empty, assign.Target, moveNextMember.ReturnType);

            var callDispose = new BoundCallEx(disposeMember, ImmutableArray<BoundArgument>.Empty,
                ImmutableArray<ITypeSymbol>.Empty, assign.Target, Compilation.CoreTypes.Void.Symbol);

            var bindInfo = new ForeachBindInfo()
            {
                CurrentMember = currentMember,
                EnumeratorSymbol = enumeratorSymbol,
                GetEnumerator = getEnumMethod,
                MoveNextMember = moveNextMember,
                DisposeMember = disposeMember,
                DisposeCall = callDispose,
                EnumeratorVar = assign.Target,
                EnumeratorAssignmentEx = assign,
                MoveNextEx = moveNextCondition,
            };

            return new BoundForEachStmt(BindVariable(localVar), collection, bindInfo);
        }

        private BoundVariableRef BindVar(SyntaxToken identifier, TypeSymbol type)
        {
            var local = Method.LocalsTable.BindLocalVariable(new VariableName(identifier.Text), identifier.Span, type);
            var boundVar = BindVariable(local).WithAccess(BoundAccess.ReadAndWrite);
            return boundVar;
        }

        private BoundAssignEx CreateTmpAndAssign(TypeSymbol type, BoundExpression expr)
        {
            var local = Method.LocalsTable.BindTemporalVariable(new VariableName(NextTempVariableName()), type);
            var boundVar = BindVariable(local).WithAccess(BoundAccess.ReadAndWrite);
            return new BoundAssignEx(boundVar, expr).WithAccess(BoundAccess.ReadAndWrite);
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

        public BoundStatement BindEmptyStmt(TextSpan span)
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

            switch (expr)
            {
                case LiteralEx ex:
                    return BindLiteralEx(ex).WithAccess(access);
                case NameEx ne:
                    return BindNameEx(ne, access);
                case BinaryEx bex:
                    return BindBinaryEx(bex).WithAccess(access);
                case AssignEx aex:
                    return BindAssignEx(aex, access);
                case InvocationEx ce:
                    return BindCallEx(ce).WithAccess(access);
                case MatchEx me:
                    return BindMatchEx(me).WithAccess(access);
                case MemberAccessEx mae:
                    return BindMemberAccessEx(mae, new NameBindingContext()).WithAccess(access);
                case ElementAccessEx eae:
                    return BindIndexerEx(eae).WithAccess(access);
                case PostfixUnaryEx pue:
                    return BindPostfixUnaryEx(pue).WithAccess(access);
                case PrefixUnaryEx prue:
                    return BindPrefixUnaryEx(prue).WithAccess(access);
                case ThrowEx throwEx:
                    return new BoundThrowEx(BindExpression(throwEx.Expression, BoundAccess.Read), null);
                case AllocEx allocEx:
                    return BindAllocEx(allocEx).WithAccess(access);
                case ParenthesizedEx pe:
                    return BindExpression(pe.Expression).WithAccess(access);
                case FuncEx fex:
                    return BindFuncEx(fex).WithAccess(BoundAccess.Read);
                default:
                    Diagnostics.Add(GetLocation(expr), ErrorCode.ERR_NotYetImplemented,
                        $"Expression of type '{expr.GetType().Name}'");

                    return new BoundBadEx(this.Compilation.GetSpecialType(SpecialType.System_Void));
            }
        }

        private BoundExpression BindFuncEx(FuncEx funcEx)
        {
            var nestedType = new SynthesizedTypeSymbol(this.Container, this.Compilation);
            var lambda = new SourceLambdaSymbol(nestedType, funcEx);
            return new BoundFuncEx(nestedType, lambda, lambda.ReturnType);
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
                SyntaxKind.GreaterThanExpression => Operations.GreaterThan,
                SyntaxKind.LessThanExpression => Operations.LessThan,
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

                case SyntaxKind.StringKeyword:
                    return new BoundConversionEx(boundOperation, PrimitiveBoundTypeRefs.StringTypeRef, null);

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


            if (boundExpr is not BoundReferenceEx target)
            {
                Diagnostics.Add(GetLocation(expr), ErrorCode.ERR_TypeNameCannotBeResolved,
                    $"Can't assign to not reference expression");

                return new BoundBadEx(boundExpr.Type).WithSyntax(expr);
            }

            BoundExpression value;

            value = BindExpression(expr.Right, BoundAccess.Read);

            var op = ToOp(expr.Kind());

            if (op is Operations.AssignValue or Operations.AssignRef)
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
            
            return BindMethod(expr.Expression, new NameBindingContext(expr.ArgumentList));
        }

        private BoundExpression BindMethod(ExprSyntax expr, NameBindingContext context)
        {
            return expr.Kind() switch
            {
                SyntaxKind.IdentifierEx => BindName((SimpleNameEx)expr, context),
                SyntaxKind.SimpleMemberAccessExpression => BindMemberAccessEx((MemberAccessEx)expr, context),
                SyntaxKind.GenericName => BindName((GenericEx)expr, context),
                _ => throw new NotImplementedException()
            };
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

                for (var i = 0; i < methods.Length; i++)
                {
                    if (methods[i].Arity == typeArgs.Length) // TODO: check the type argument is assignable
                    {
                        result.Add(methods[i].Construct(typeArgs.ToArray()));
                    }
                }

                return result.ToImmutableArray();
            }
        }


        protected class NameBindingContext
        {
            private ImmutableArray<BoundArgument> _boundedArguments;

            public NameBindingContext()
            {
            }

            public NameBindingContext(ArgumentListSyntax argumentList)
            {
                Arguments = argumentList.Arguments;
                IsInvocation = true;
            }

            public bool IsInvocation { get; }

            public SeparatedSyntaxList<ArgumentSyntax> Arguments { get; }

            public ImmutableArray<BoundArgument> BindArguments(Binder binder)
            {
                if (!IsInvocation)
                    return ImmutableArray<BoundArgument>.Empty;

                if (_boundedArguments.IsDefault)
                {
                    _boundedArguments =
                        Arguments
                            .Select(x => BoundArgument.Create(binder.BindExpression(x.Expression)))
                            .ToImmutableArray();
                }

                return _boundedArguments;
            }
        }


        protected virtual BoundExpression BindName(SimpleNameEx expr, NameBindingContext context)
        {
            Debug.Assert(Method != null);
            Debug.Assert(!string.IsNullOrEmpty(expr.Identifier.Text));

            var name = expr.Identifier.Text;

            BoundExpression CantResolve()
            {
                Diagnostics.Add(GetLocation(expr), ErrorCode.ERR_CantResolveSymbol, name);
                return new BoundBadEx(this.Compilation.GetSpecialType(SpecialType.System_Void));
            }

            var typeArgs = ImmutableArray<ITypeSymbol>.Empty;
            if (expr is GenericEx genEx)
            {
                typeArgs = genEx
                    .TypeArgumentList
                    .Arguments
                    .Select(BindType)
                    .OfType<ITypeSymbol>()
                    .ToImmutableArray();
            }

            

            if (context.IsInvocation)
            {
                if (Locals.TryGetVariable(name, out var variable))
                {
                    throw new NotImplementedException();
                }

                var containerMembers = Container.GetMembers(name).ToImmutableArray();    
                if (containerMembers.Any())
                {
                    var args = context.BindArguments(this);
                    var methods = containerMembers.OfType<MethodSymbol>().ToImmutableArray();
                    var resolved = methods switch
                    {
                        { Length: 1 } => methods[0],
                        { Length: > 1 } => (MethodSymbol)ResolveOverload(
                            containerMembers.OfType<MethodSymbol>().ToImmutableArray(), args)
                    };

                    if (resolved is null)
                    {
                        return CantResolve();
                    }

                    resolved = ConstructSingle(resolved, typeArgs);
                    var thisPlace = resolved.HasThis
                        ? new BoundVariableRef(QualifiedName.This.Name.Value, Container as ITypeSymbol)
                        : null;
                    return new BoundCallEx(resolved, context.BindArguments(this), typeArgs, thisPlace, resolved.ReturnType);
                }
            }

            if (!context.IsInvocation)
                return new BoundPropertyRef(new MissingPropertySymbol(name), null);

            var argList = context.BindArguments(this);

            var foundMethods = new ArrayBuilder<Symbol>();
            FindMethodsByName(name, foundMethods);

            var globalMembers = foundMethods.OfType<MethodSymbol>().Where(x => x.Arity == typeArgs.Count())
                .ToImmutableArray();

            globalMembers = Construct(globalMembers, typeArgs);

            var overload = (MethodSymbol)ResolveOverload(globalMembers, argList);

            if (overload == null || overload.HasThis)
            {
                return CantResolve();
            }

            return new BoundCallEx(overload, argList, typeArgs, null, overload.ReturnType);
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

            return candidates.Any() ? candidates.MinBy(x => x.Value).Key : null;
        }

        private BoundExpression BindMemberAccessEx(MemberAccessEx expr, NameBindingContext context)
        {
            var boundLeft = BindExpression(expr.Expression).WithAccess(BoundAccess.Invoke);

            var leftType = boundLeft.Type;

            var identifierText = expr.Name.GetUnqualifiedName().Identifier.Text;
            var members = leftType.GetMembers(identifierText);
            var typeArgs = ImmutableArray<ITypeSymbol>.Empty;

            if (expr.Name.Kind() == SyntaxKind.GenericName)
            {
                typeArgs = ((GenericEx)expr.Name).TypeArgumentList.Arguments.Select(BindType).OfType<ITypeSymbol>()
                    .ToImmutableArray();
            }

            ISymbol member = members switch
            {
                { Length: 1 } => members[0],
                { Length: > 1 } => ResolveOverload(members.OfType<MethodSymbol>().ToImmutableArray(),
                    context.BindArguments(this)),
                _ => null
            };

            if (member is null)
            {
                Diagnostics.Add(GetLocation(expr.Name), ErrorCode.ERR_MethodNotFound, identifierText, leftType.Name);
                return new BoundBadEx(this.Compilation.GetSpecialType(SpecialType.System_Void));
            }

            if (context.IsInvocation)
            {
                if (member is MethodSymbol ms)
                {
                    ms = ConstructSingle(ms, typeArgs);
                    return new BoundCallEx(ms, context.BindArguments(this), typeArgs,
                        (ms.IsStatic) ? null : boundLeft, ms.ReturnType);
                }

                Diagnostics.Add(GetLocation(expr.Name), ErrorCode.ERR_MethodNotFound, identifierText, leftType.Name);
            }
            else
            {
                switch (member)
                {
                    case PropertySymbol ps:
                        return new BoundPropertyRef(ps, boundLeft);
                    case FieldSymbol fs:
                        return new BoundFieldRef(fs, boundLeft);
                }
            }
            

           

            return new BoundBadEx(this.Compilation.GetSpecialType(SpecialType.System_Void));
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