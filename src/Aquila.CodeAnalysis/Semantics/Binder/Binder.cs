using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Aquila.CodeAnalysis.Errors;
using Aquila.CodeAnalysis.Semantics.TypeRef;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Syntax;
using Aquila.CodeAnalysis.Utilities;
using Aquila.Core.Querying.Model;
using Aquila.Syntax.Ast;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

        protected Binder(AquilaCompilation compilation)
        {
            Compilation = compilation;
        }

        protected Binder(Binder next)
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

        public virtual NamespaceOrTypeSymbol ContainingType { get; }

        public LocalsTable Locals => Method.LocalsTable;

        public virtual ImmutableArray<TypeSymbol> DeclaredTypes => ImmutableArray<TypeSymbol>.Empty;

        public virtual bool IsInLambda => false;

        public abstract Symbol ContainingSymbol { get; }

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

            var localVar = Locals.BindLocalVariable(new VariableName(decl.Identifier.Text), decl);

            var boundExpression = BindExpression(decl.Initializer.Value);

            return new BoundExpressionStmt(new BoundAssignEx(
                new BoundVariableRef(decl.Identifier.Text, localVar.Type).WithAccess(BoundAccess.Write),
                boundExpression, localVar.Type));
        }

        private BoundStatement BindVarDeclStmt(LocalDeclStmt varDecl)
        {
            return BindVarDecl(varDecl.Declaration);
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

        public TypeSymbol BindType(TypeEx typeEx)
        {
            var trf = Compilation.CoreTypes;

            switch (typeEx)
            {
                case PredefinedTypeEx pt:
                    return pt.Keyword.Kind() switch
                    {
                        SyntaxKind.IntKeyword => trf.Int32,
                        SyntaxKind.LongKeyword => trf.Int64,
                        SyntaxKind.VoidKeyword => trf.Void,
                        SyntaxKind.ObjectKeyword => trf.Object,
                        SyntaxKind.StringKeyword => trf.String,
                        SyntaxKind.BoolKeyword => trf.Boolean,
                        SyntaxKind.DatetimeKeyword => trf.DateTime,
                        _ => throw ExceptionUtilities.UnexpectedValue(pt.Kind())
                    };
                case NameEx named:
                {
                    var typeSymbol = FindTypeByName(named);
                    return (TypeSymbol)typeSymbol;
                }
                case UnionTypeEx union:
                {
                    var listSym = union.Types.Select(BindType).ToList();
                    var tsym = Compilation.PlatformSymbolCollection.SynthesizeUnionType(
                        Compilation.SourceModule.GlobalNamespace, listSym);

                    return tsym;
                }
                default:
                    Diagnostics.Add(GetLocation(typeEx), ErrorCode.ERR_NotYetImplemented,
                        $"Expression of type '{typeEx.GetType().Name}'");
                    throw new NotImplementedException();
            }
        }

        private ImmutableArray<ImmutableArray<Symbol>> FindSymbolByName(string name, params SymbolKind[] kinds)
        {
            var foundSymbolsBuilder = new ArrayBuilder<ImmutableArray<Symbol>>();
            FindSymbolByName(name, foundSymbolsBuilder, new FilterCriteria { SymbolKinds = kinds });
            return foundSymbolsBuilder.ToImmutableAndFree();
        }

        protected virtual void FindSymbolByName(string name, ArrayBuilder<ImmutableArray<Symbol>> result,
            FilterCriteria filterCriteria)
        {
            Next?.FindSymbolByName(name, result, filterCriteria);
        }

        protected void FindSymbolByNameHandler(IEnumerable<Symbol> symbols, ArrayBuilder<ImmutableArray<Symbol>> result,
            FilterCriteria criteria)
        {
            if (criteria.SymbolKinds.Any())
            {
                symbols = symbols.Where(s => criteria.SymbolKinds.Contains(s.Kind));
            }

            result.Add(symbols.ToImmutableArray());
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
                    return BindMemberAccessEx(mae).WithAccess(access);
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
            var lambda = Locals.BindLambda(funcEx);
            return new BoundFuncEx(lambda, lambda.ReturnType);
        }

        private BoundExpression BindAllocEx(AllocEx ex)
        {
            var type = (NamedTypeSymbol)BindType(ex.Name);
            var inits = new List<BoundAllocExAssign>();

            if (!ex.Initializer.Expressions.Any()) return new BoundAllocEx(type, inits, type);

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
                        switch (mems.Length)
                        {
                            case > 1:
                                //ambiguity
                                break;
                            case 1:
                            {
                                var item = mems[0];

                                inits.Add(new BoundAllocExAssign(item, right));
                                break;
                            }
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

            foreach (var arm in me.Arms)
            {
                var boundArm = BindMatchArm(arm);

                arms.Add(boundArm);

                var armType = (TypeSymbol)(boundArm.ResultType);

                if (armType == null) continue;
                // then we try to calc result type

                if (resultType != null)
                {
                    if (resultType == armType) continue;

                    if (resultType.IsEqualToOrDerivedFrom(armType))
                    {
                        //error
                    }

                    if (armType.IsEqualToOrDerivedFrom(resultType))
                    {
                        //cast
                    }
                }
                else
                {
                    resultType = armType;
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

            if(!TryResolveSymbol(expr, access, out var result))
            {
                return BadEx;
            }
            
            var boundThis = new BoundVariableRef("this", ContainingType as ITypeSymbol).WithAccess(BoundAccess.Read);
            var resultSymbol = result.Symbol;
            switch (resultSymbol.Kind)
            {
                case SymbolKind.Local:
                case SymbolKind.Parameter:
                    return new BoundVariableRef(resultSymbol.Name, resultSymbol.GetTypeOrReturnType());
                case SymbolKind.Field:
                    return new BoundFieldRef(resultSymbol as IFieldSymbol, boundThis);
                case SymbolKind.Property:
                    return new BoundPropertyRef(resultSymbol as IPropertySymbol, boundThis);
                case SymbolKind.NamedType:
                    return new BoundClassTypeRef(QualifiedName.Object, Method, resultSymbol as ITypeSymbol);
                case SymbolKind.Method:
                    return new BoundMethodRef(resultSymbol as MethodSymbol, boundThis,
                        resultSymbol.GetTypeOrReturnType());
            }

            return BadEx;
        }

        private BoundExpression BadEx => new BoundBadEx(Compilation.CoreTypes.Void.Symbol);

        private bool TryResolveSymbol(NameEx nameEx, BoundAccess access, out (Symbol Symbol, bool IsCaptured) result)
        {
            var name = nameEx.GetUnqualifiedName().Identifier.Text;

            var foundSymbolsByBinders = FindSymbolByName(name);

            if (!foundSymbolsByBinders.Any())
            {
                Diagnostics.Add(GetLocation(nameEx), ErrorCode.ERR_UndefinedIdentifier, name);
                result = (null, false);
                return false;
            }

            for (var index = 0; index < foundSymbolsByBinders.Length; index++)
            {
                var binderSymbols = foundSymbolsByBinders[index];
                var isCaptured = index != 0 && IsInLambda;

                switch (binderSymbols)
                {
                    case { Length: 0 }: continue;
                    case { Length: 1 }:
                        var resolvedSymbol = binderSymbols[0];
                        if (resolvedSymbol is MethodSymbol method)
                        {
                            resolvedSymbol = ConstructSingle(nameEx, method, BindTypeArguments(nameEx));
                        }

                        result = (resolvedSymbol, isCaptured);
                        return true;
                    case { Length: > 1 } when binderSymbols.All(x => x is MethodSymbol) && access.IsInvoke:
                        var resolved1 = (MethodSymbol)ResolveOverload(
                            binderSymbols.Cast<MethodSymbol>().ToImmutableArray(), 
                            access.Arguments);
                        result = (resolved1, isCaptured);
                        return true;
                    case { Length: > 1 }:
                        Diagnostics.Add(GetLocation(nameEx), ErrorCode.ERR_AmbiguousName, name);
                        result = (null, false);
                        return false;
                }
            }

            Diagnostics.Add(nameEx.GetLocation(), ErrorCode.ERR_CantResolveSymbol, name);
            result = (null, false);
            return false;
        }

        protected BoundExpression BindSimpleVarUse(NameEx expr, BoundAccess access)
        {
            var varName = BindVariableName(expr);

            // variable not found
            if (varName is null) return null;

            if (Method == null)
            {
                // cannot use variables in field initializer or parameter initializer
                Diagnostics.Add(GetLocation(expr), ErrorCode.ERR_InvalidConstantExpression);
            }

            if (!varName.NameValue.IsThisVariableName)
                return new BoundVariableRef(varName, varName.Type).WithAccess(access);

            if (access.IsEnsure)
                access = BoundAccess.Read;

            if (access.IsWrite || access.IsUnset)
                Diagnostics.Add(GetLocation(expr), ErrorCode.ERR_CannotAssignToThis);

            if (Method != null && Method.IsStatic && !Method.IsGlobalScope)
            {
                //TODO: Add diagnostics
                throw new NotImplementedException();
            }

            return new BoundVariableRef(varName, varName.Type).WithAccess(access);
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

            if (!Locals.TryGetVariable(new VariableName(varName), out var localVar))
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

        private BoundExpression BindCallEx(InvocationEx call)
        {
            var method = call.Expression;
            var boundArguments = BindArguments(call.ArgumentList);
            var access = BoundAccess.Invoke.WithArguments(boundArguments);
            return method.Kind() switch
            {
                SyntaxKind.IdentifierEx => BindNameEx((NameEx)method, access),
                SyntaxKind.SimpleMemberAccessExpression => BindMemberAccessEx((MemberAccessEx)method, access),
                SyntaxKind.GenericName => BindNameEx((GenericEx)method, BoundAccess.Invoke),
                _ => throw new NotImplementedException()
            };
        }

        MethodSymbol ConstructSingle(SyntaxNode node, MethodSymbol method, ImmutableArray<ITypeSymbol> typeArgs)
        {
            if (typeArgs.IsDefaultOrEmpty)
            {
                return method;
            }

            if (method.Arity == typeArgs.Length)
            {
                return method.Construct(typeArgs.ToArray());
            }

            Diagnostics.Add(node.GetLocation(), ErrorCode.ERR_BadArity, method.Name, method.Arity);
            return new MissingMethodSymbol("");
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

        protected abstract class BindingContext
        {
            private readonly Binder _binder;
            private BoundExpression _instance;

            protected BindingContext(Binder binder)
            {
                _binder = binder;
            }

            public abstract bool IsInvocation { get; }

            public BoundExpression Instance => _instance;
        }

        protected sealed class SimpleBindingContext : BindingContext
        {
            public SimpleBindingContext(Binder binder) : base(binder)
            {
            }

            public override bool IsInvocation => false;
        }


        protected sealed class CallBindingContext : BindingContext
        {
            private readonly ExprSyntax _expression;
            private readonly Binder _binder;
            private ImmutableArray<BoundArgument> _boundedArguments;
            private ImmutableArray<ITypeSymbol> _boundTypeArguments;

            public CallBindingContext(Binder binder) : base(binder)
            {
                _binder = binder;
            }

            public CallBindingContext(Binder binder, ExprSyntax expression, ArgumentListSyntax argumentList) :
                this(binder)
            {
                _expression = expression;
                Arguments = argumentList.Arguments;
            }

            public override bool IsInvocation => true;

            public SeparatedSyntaxList<ArgumentSyntax> Arguments { get; }

            public BoundVariableRef BindThis(MethodSymbol symbol)
            {
                return symbol.HasThis && symbol.ContainingType == _binder.ContainingType
                    ? new BoundVariableRef(QualifiedName.This.Name.Value, _binder.ContainingType as ITypeSymbol)
                    : null;
            }

            public ImmutableArray<BoundArgument> BindArguments()
            {
                if (!IsInvocation)
                    return ImmutableArray<BoundArgument>.Empty;

                if (!_boundedArguments.IsDefault) return _boundedArguments;
                return _boundedArguments =
                    Arguments
                        .Select(x => BoundArgument.Create(_binder.BindExpression(x.Expression)))
                        .ToImmutableArray();
            }

            public ImmutableArray<ITypeSymbol> BindTypeArguments()
            {
                if (_expression is not GenericEx genEx)
                    return ImmutableArray<ITypeSymbol>.Empty;

                if (_boundTypeArguments.IsDefault)
                    _boundTypeArguments = genEx
                        .TypeArgumentList
                        .Arguments
                        .Select(x => _binder.BindType(x))
                        .OfType<ITypeSymbol>()
                        .ToImmutableArray();

                return _boundTypeArguments;
            }
        }

        public ImmutableArray<ITypeSymbol> BindTypeArguments(NameEx nameExpr)
        {
            if (nameExpr is not GenericEx genEx)
                return ImmutableArray<ITypeSymbol>.Empty;
            return genEx
                .TypeArgumentList
                .Arguments
                .Select(BindType)
                .OfType<ITypeSymbol>()
                .ToImmutableArray();
        }

        public ImmutableArray<BoundArgument> BindArguments(ArgumentListSyntax argumentListSyntax)
        {
            return argumentListSyntax.Arguments
                .Select(x => BoundArgument.Create(BindExpression(x.Expression)))
                .ToImmutableArray();
        }

        protected virtual BoundExpression BindName(SimpleNameEx expr, BindingContext context)
        {
            Debug.Assert(Method != null);
            Debug.Assert(!string.IsNullOrEmpty(expr.Identifier.Text));

            var name = expr.Identifier.Text;

            if (!context.IsInvocation || context is not CallBindingContext cbc)
                return new BoundBadEx(this.Compilation.GetSpecialType(SpecialType.System_Void));

            foreach (var binderSymbols in FindSymbolByName(name))
            {
                switch (binderSymbols)
                {
                    case { Length: 0 }: continue;
                    case { Length: 1 }:

                        switch (binderSymbols[0])
                        {
                            case { Kind: SymbolKind.Parameter } c when c.GetTypeOrReturnType().IsDelegateType():
                            {
                                if (!Locals.TryGetVariable(c.Name, out var v))
                                {
                                    throw new NotImplementedException();
                                }

                                var delegateType = c.GetTypeOrReturnType().DelegateInvokeMethod();
                                return new BoundCallEx(delegateType,
                                    cbc.BindArguments(), cbc.BindTypeArguments(),
                                    new BoundVariableRef(v.BoundName, c.GetTypeOrReturnType()),
                                    delegateType.ReturnType);
                            }
                            case { Kind: SymbolKind.Local } l: throw new NotImplementedException();
                        }

                        break;
                    case { Length: > 1 } when binderSymbols.All(x => x is MethodSymbol):
                        var resolved1 = (MethodSymbol)ResolveOverload(
                            binderSymbols.Cast<MethodSymbol>().ToImmutableArray(),
                            cbc.BindArguments());

                        if (resolved1 is null)
                            throw new NotImplementedException("TODO: add diagnostics error message");

                        return new BoundCallEx(resolved1,
                            cbc.BindArguments(), cbc.BindTypeArguments(), cbc.BindThis(resolved1),
                            resolved1.ReturnType);
                    case { Length: > 1 }:
                        throw new NotImplementedException(
                            "Ambiguous symbol. Can't declare different type of symbols with the same name in scope");
                }
            }

            var containerMembers = ContainingType.GetMembers(name);
            if (containerMembers.Any())
            {
                var args = cbc.BindArguments();
                var methods = containerMembers.OfType<MethodSymbol>().ToImmutableArray();
                var resolved = methods switch
                {
                    { Length: 1 } => methods[0],
                    { Length: > 1 } => (MethodSymbol)ResolveOverload(
                        containerMembers.OfType<MethodSymbol>().ToImmutableArray(), args),
                    _ => null
                };

                if (resolved is null)
                {
                    return CantResolve();
                }

                resolved = ConstructSingle(resolved, cbc.BindTypeArguments());

                return new BoundCallEx(resolved,
                    cbc.BindArguments(),
                    cbc.BindTypeArguments(),
                    cbc.BindThis(resolved),
                    resolved.ReturnType);
            }

            var argList = cbc.BindArguments();

            var foundMethods = new ArrayBuilder<Symbol>();
            FindMethodsByName(name, foundMethods);

            var globalMembers = foundMethods.OfType<MethodSymbol>()
                .Where(x => x.Arity == cbc.BindTypeArguments().Count())
                .ToImmutableArray();

            globalMembers = Construct(globalMembers, cbc.BindTypeArguments());

            var overload = (MethodSymbol)ResolveOverload(globalMembers, argList);
            if (overload == null || overload.HasThis)
            {
                return CantResolve();
            }

            return new BoundCallEx(overload, argList, cbc.BindTypeArguments(), null, overload.ReturnType);
        }

        private static IMethodSymbol ResolveOverload(ImmutableArray<MethodSymbol> overloads,
            ImmutableArray<BoundArgument> argsList)
        {
            var types = argsList.Select(x => x.Type).ToImmutableArray();

            Dictionary<MethodSymbol, int> candidates = new();

            if (overloads.Length <= 0) return candidates.Any() ? candidates.MinBy(x => x.Value).Key : null;

            foreach (var methodSymbol in overloads)
            {
                var cost = 0;

                var parameters = methodSymbol.Parameters;

                var paramPlace = 0;

                if (methodSymbol.HasParamPlatformContext)
                {
                    paramPlace = 1;
                }

                if (types.Length != parameters.Length - paramPlace)
                    continue;

                var isCandidate = true;

                for (int i = 0; i < types.Length; i++)
                {
                    var leftType = types[i];
                    var rightType = parameters[i + paramPlace].Type;

                    if (!leftType.IsEqualOrDerivedFrom(rightType))
                    {
                        isCandidate = false;
                        break;
                    }

                    // if types not equal, then we need use boxing - it expensive
                    cost += SymbolEqualityComparer.Default.Equals(leftType, rightType) ? 1 : 2;
                }

                if (isCandidate)
                    candidates[methodSymbol] = cost;
            }

            return candidates.Any() ? candidates.MinBy(x => x.Value).Key : null;
        }

        private BoundExpression BindMemberAccessEx(MemberAccessEx expr, BoundAccess access)
        {
            var boundLeft = BindExpression(expr.Expression);

            var leftType = boundLeft.Type;

            var binder = new InContainerBinder(leftType, new BuckStopsHereBinder(Compilation));
            ;

            if (!binder.TryResolveSymbol(expr.Name, new SimpleBindingContext(binder), out var result))
                return BadEx;

            var (symbol, _) = result;

            if (!SymbolEqualityComparer.Default.Equals(leftType, symbol.ContainingType))
                return BadEx;

            switch (symbol)
            {
                case PropertySymbol ps:
                    return new BoundPropertyRef(ps, boundLeft);
                case FieldSymbol fs:
                    return new BoundFieldRef(fs, boundLeft);
            }

            return BadEx;
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

    internal sealed class FilterCriteria
    {
        public SymbolKind[] SymbolKinds { get; init; }
    }
}