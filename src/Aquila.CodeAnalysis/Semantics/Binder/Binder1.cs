using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.Symbols;
using Roslyn.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Aquila.CodeAnalysis.Errors;
using Aquila.CodeAnalysis.FlowAnalysis;
using Aquila.CodeAnalysis.Semantics.Graph;
using Aquila.Syntax;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Ast.Expressions;
using Aquila.Syntax.Ast.Functions;
using Aquila.Syntax.Ast.Statements;
using Aquila.Syntax.Errors;
using Aquila.Syntax.Syntax;
using Aquila.Syntax.Text;

namespace Aquila.CodeAnalysis.Semantics
{
    /// <summary>
    /// Binds syntax nodes (<see cref="LangElement"/>) to semantic nodes (<see cref="IOperation"/>).
    /// Creates unbound nodes.
    /// </summary>
    internal class Binder1
    {
        /// <summary>
        /// Optional. Local variables table.
        /// Can be <c>null</c> for expressions without variable access (field initializers and parameters initializers).
        /// </summary>
        protected readonly LocalsTable Locals;

        /// <summary>
        /// Optional. Self type context.
        /// </summary>
        public TypeSymbol Self { get; }

        /// <summary>
        /// Containing file symbol.
        /// </summary>
        public SyntaxTree ContainingFile { get; }

        /// <summary>
        /// Gets corresponding method.
        /// Can be <c>null</c>.
        /// </summary>
        public SourceMethodSymbol Method => _method ?? Locals?.Method;

        readonly SourceMethodSymbol _method;

        /// <summary>
        /// Found yield statements (needed for ControlFlowGraph)
        /// </summary>
        public virtual ImmutableArray<BoundYieldStmt> Yields
        {
            get => ImmutableArray<BoundYieldStmt>.Empty;
        }

        /// <summary>
        /// Generated state machine states.
        /// </summary>
        public virtual int StatesCount => 0;

        /// <summary>
        /// Bag with semantic diagnostics.
        /// </summary>
        public DiagnosticBag Diagnostics => DeclaringCompilation.DeclarationDiagnostics;

        /// <summary>
        /// Compilation.
        /// </summary>
        internal AquilaCompilation DeclaringCompilation { get; }

        /// <summary>Gets <see cref="BoundTypeRefFactory"/> instance.</summary>
        internal BoundTypeRefFactory BoundTypeRefFactory => DeclaringCompilation.TypeRefFactory;

        /// <summary>
        /// Gets value determining whether to compile <c>assert</c>. otherwise the expression is ignored.
        /// </summary>
        bool EnableAssertExpression => Method != null && Method.DeclaringCompilation.Options.DebugPlusMode;

        #region Construction

        /// <summary>
        /// Creates <see cref="Binder1"/> for given method (passed with <paramref name="locals"/>).
        /// </summary>
        /// <param name="file">Containing file.</param>
        /// <param name="locals">Table of local variables within method.</param>
        /// <param name="self">Current self context.</param>
        /// <param name="compilation">Declaring compilation.</param>
        public static Binder1 Create(AquilaCompilation compilation, SyntaxTree file, LocalsTable locals,
            object self = null)
        {
            Debug.Assert(locals != null);

            var method = locals.Method;
            Debug.Assert(method != null);

            return new Binder1(compilation, file, locals, method, self);
        }

        public Binder1(AquilaCompilation compilation, SyntaxTree file, LocalsTable locals = null,
            SourceMethodSymbol method = null, object self = null)
        {
            DeclaringCompilation = compilation;

            ContainingFile = file;
            Locals = locals;
            _method = method;
            Self = (TypeSymbol)self;
        }

        /// <summary>
        /// Provides binder with CFG builder.
        /// </summary>
        public virtual void SetupBuilder(Func<BoundBlock> newBlockFunc)
        {
        }

        #endregion

        #region Helpers

        protected ImmutableArray<BoundStatement> BindStatements(IEnumerable<Statement> statements)
        {
            return statements.Select(BindStatement).ToImmutableArray();
        }

        protected ImmutableArray<BoundExpression> BindExpressions(IEnumerable<Expression> expressions)
        {
            return expressions.Select(BindExpression).ToImmutableArray();
        }

        protected BoundExpression BindExpression(Expression expr) => BindExpression(expr, BoundAccess.Read);

        protected BoundArgument BindArgument(Expression expr, bool isByRef = false, bool isUnpack = false)
        {
            //if (isUnpack)
            //{
            //    // remove:
            //    _diagnostics.Add(_locals.Method, expr, Errors.ErrorCode.ERR_NotYetImplemented, "Parameter unpacking");
            //}

            var bound = BindExpression(expr, isByRef ? BoundAccess.ReadRef : BoundAccess.Read);
            Debug.Assert(!isUnpack || !isByRef);

            return isUnpack
                ? BoundArgument.CreateUnpacking(bound)
                : BoundArgument.Create(bound);
        }

        protected ImmutableArray<BoundArgument> BindArguments(params Expression[] expressions)
        {
            Debug.Assert(expressions != null);

            if (expressions.Length == 0)
            {
                return ImmutableArray<BoundArgument>.Empty;
            }

            //
            var arguments = new BoundArgument[expressions.Length];
            for (int i = 0; i < expressions.Length; i++)
            {
                var expr = expressions[i];
                arguments[i] = BindArgument(expr);
            }

            //
            return ImmutableArray.Create(arguments);
        }

        protected ImmutableArray<BoundArgument> BindArguments(params Argument[] parameters)
        {
            Debug.Assert(parameters != null);

            var pcount = parameters.Length;
            while (pcount > 0 && parameters[pcount - 1].Expression == null)
            {
                pcount--;
            }

            //
            if (pcount == 0)
            {
                return ImmutableArray<BoundArgument>.Empty;
            }

            //
            var unpacking = false;
            var arguments = new BoundArgument[pcount];
            for (int i = 0; i < pcount; i++)
            {
                var p = parameters[i];
                var arg = BindArgument(p.Expression, false, false);

                //
                arguments[i] = arg;

                // check the unpacking is not used before normal arguments:
                if (arg.IsUnpacking)
                {
                    unpacking = true;
                }
                else
                {
                    if (unpacking)
                    {
                        Diagnostics.Add(GetLocation(p), ErrorCode.ERR_PositionalArgAfterUnpacking);
                    }
                }
            }

            //
            return ImmutableArray.Create(arguments);
        }

        // protected ImmutableArray<BoundArgument> BindLambdaUseArguments(IList<FormalParam> usevars)
        // {
        //     if (usevars != null && usevars.Count != 0)
        //     {
        //         return usevars.SelectAsArray(v =>
        //         {
        //             var varuse = new DirectVarUse(v.Name.Span, v.Name.Name);
        //             var boundvar = BindExpression(varuse, v.PassedByRef ? BoundAccess.ReadRef : BoundAccess.Read);
        //
        //             return BoundArgument.Create(boundvar);
        //         });
        //     }
        //     else
        //     {
        //         return ImmutableArray<BoundArgument>.Empty;
        //     }
        // }

        protected Location GetLocation(LangElement expr) => ContainingFile.GetLocation(expr);

        #endregion

        public virtual void WithTryScopes(IEnumerable<TryCatchEdge> tryScopes)
        {
        }

        public virtual BoundItemsBag<BoundStatement> BindWholeStatement(Statement stmt) => BindStatement(stmt);

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

        protected BoundStatement BindReturnStmt(ReturnStmt stmt)
        {
            Debug.Assert(Method != null);

            BoundExpression expr = null;

            if (stmt.Expression != null)
            {
                expr = BindExpression(stmt.Expression, BoundAccess.Read);

                // if (!isByRef)
                // {
                //     // copy returned value
                //     expr = BindCopyValue(expr);
                // }
            }

            //
            return new BoundReturnStmt(expr).WithSyntax(expr.AquilaSyntax);
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


        public virtual BoundItemsBag<BoundExpression> BindWholeExpression(Expression expr, BoundAccess access) =>
            BindExpression(expr, access);

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
                return new BoundThrowEx(BindExpression(throwEx.Expression, BoundAccess.Read), null);

            //
            Diagnostics.Add(GetLocation(expr), ErrorCode.ERR_NotYetImplemented,
                $"Expression of type '{expr.GetType().Name}'");
            return new BoundLiteral(null, null);
        }

        private BoundMethodName BindMethodName(NameEx name)
        {
            return new BoundMethodName(QualifiedName.Parse(name.Identifier.Text, true));
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
                                arglist, ImmutableArray<IBoundTypeRef>.Empty, null);
                        }
                        else
                        {
                            var th = new BoundVariableRef("this");

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
                        throw new Exception("Can't bound property: not implemented");
                    }
                }
            }

            throw new NotImplementedException();
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


        private BoundExpression BindCallEx(CallEx expr)
        {
            return BindMethodGroup(expr.Expression, expr.Arguments, true);
        }

        protected BoundExpression BindIncDec(IncDecEx expr)
        {
            // bind variable reference
            var varref = (BoundReferenceEx)BindExpression(expr.Operand, BoundAccess.ReadAndWrite);

            //
            return new BoundIncDecEx(varref, expr.IsIncrement, expr.IsPost, null);
        }

        protected BoundVariableName BindVariableName(NameEx nameEx)
        {
            Debug.Assert(nameEx != null);

            return new BoundVariableName(nameEx.Identifier.Text).WithSyntax(nameEx);
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

                return new BoundVariableRef(varname, null).WithAccess(access);
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
                        expr.Operation, null);
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
                                .StringTypeRef,
                            null); // TODO // CONSIDER: should be WritableString and analysis should rewrite it to String if possible

                case Operations.BinaryCast:
                    return new BoundConversionEx(
                        new BoundConversionEx(
                            boundOperation,
                            BoundTypeRefFactory.Create(
                                DeclaringCompilation.CreateArrayTypeSymbol(
                                    DeclaringCompilation.GetSpecialType(SpecialType.System_Byte))), null
                        ).WithAccess(BoundAccess.Read),
                        BoundTypeRefFactory.StringTypeRef, null);

                case Operations.ArrayCast:
                    return new BoundConversionEx(boundOperation, BoundTypeRefFactory.BoolTypeRef, null);

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
                return new BoundAssignEx(target, value, null).WithAccess(access);
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
                            value = new BoundBinaryEx(new BoundLiteral(null, null).WithAccess(BoundAccess.Read), value,
                                AstUtils.CompoundOpToBinaryOp(expr.Operation), null);
                            break;
                    }

                    return new BoundAssignEx(target, value.WithAccess(BoundAccess.Read), null).WithAccess(access);
                }
                else
                {
                    target.Access = target.Access.WithRead(); // Read & Write on target

                    return new BoundCompoundAssignEx(target, value, expr.Operation, null).WithAccess(access);
                }
            }
        }

        public BoundStatement BindEmptyStmt(Span span)
        {
            return new BoundEmptyStmt(span.ToTextSpan());
        }

        protected static BoundExpression BindLiteral(LiteralEx expr)
        {
            switch (expr.Operation)
            {
                case Operations.IntLiteral:
                case Operations.StringLiteral:
                case Operations.DoubleLiteral:
                case Operations.NullLiteral:
                case Operations.BinaryStringLiteral:
                case Operations.BoolLiteral:
                    return new BoundLiteral(expr.ObjectiveValue, null);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Updates <paramref name="expr"/>'s <see cref="BoundAccess"/> to <see cref="BoundAccess.ReadRef"/>.
        /// </summary>
        /// <param name="expr">Expression which access has to be updated.</param>
        public static void BindReadRefAccess(BoundReferenceEx expr)
        {
            if (expr == null || expr.Access.IsReadRef) return;

            expr.Access = expr.Access.WithReadRef();
            BindEnsureAccess(expr); // parent expression chain has to be updated as well
        }

        /// <summary>
        /// Updates <paramref name="expr"/>'s <see cref="BoundAccess"/> to <see cref="BoundAccess.Write"/>.
        /// </summary>
        /// <param name="expr">Expression which access has to be updated.</param>
        public static void BindWriteAccess(BoundReferenceEx expr)
        {
            if (expr == null || expr.Access.IsWrite) return;

            expr.Access = expr.Access.WithWrite();
            BindEnsureAccess(expr); // parent expression chain has to be updated as well
        }

        /// <summary>
        /// Updates <paramref name="expr"/>'s <see cref="BoundAccess"/> to <see cref="BoundAccess.Write"/>.
        /// </summary>
        /// <param name="expr">Expression which access has to be updated.</param>
        public static void BindEnsureArrayAccess(BoundReferenceEx expr)
        {
            if (expr == null || expr.Access.IsWrite) return;

            expr.Access = expr.Access.WithEnsureArray();
            BindEnsureAccess(expr); // parent expression chain has to be updated as well
        }

        protected static void BindEnsureAccess(BoundExpression expr)
        {
            if (expr is BoundArrayItemEx arritem)
            {
                arritem.Array.Access = arritem.Array.Access.WithEnsureArray();
                BindEnsureAccess(arritem.Array);
            }
        }
    }

    internal sealed class GeneratorBinder : Binder1
    {
        #region FieldsAndProperties

        /// <summary>
        /// Found yield statements (needed for ControlFlowGraph)
        /// </summary>
        public override ImmutableArray<BoundYieldStmt> Yields
        {
            get => _yields.ToImmutableArray();
        }

        public override int StatesCount => _yields.Count;

        readonly List<BoundYieldStmt> _yields = new List<BoundYieldStmt>();

        readonly HashSet<LangElement> _yieldsToStatementRootPath = new HashSet<LangElement>();
        int _rewriterVariableIndex = 0;
        int _underYieldLikeExLevel = -1;

        #endregion

        #region PreBoundBlocks

        private BoundBlock _preBoundBlockFirst;
        private BoundBlock _preBoundBlockLast;

        private BoundBlock NewBlock() => _newBlockFunc();
        private Func<BoundBlock> _newBlockFunc;

        private IEnumerable<TryCatchEdge> _tryScopes;

        BoundYieldStmt NewYieldStatement(BoundExpression valueExpression, BoundExpression keyExpression,
            LangElement syntax = null,
            bool isYieldFrom = false)
        {
            var yieldStmt = new BoundYieldStmt(_yields.Count + 1, valueExpression, keyExpression, _tryScopes)
                {
                    IsYieldFrom = isYieldFrom,
                }
                .WithSyntax(syntax);

            _yields.Add(yieldStmt);

            return yieldStmt;
        }

        string GetTempVariableName()
        {
            return "<sm>" + _rewriterVariableIndex++;
        }

        void InitPreBoundBlocks()
        {
            _preBoundBlockFirst = _preBoundBlockFirst ?? NewBlock();
            _preBoundBlockLast = _preBoundBlockLast ?? _preBoundBlockFirst;
        }

        BoundBlock CurrentPreBoundBlock
        {
            get
            {
                InitPreBoundBlocks();
                return _preBoundBlockLast;
            }
            set { _preBoundBlockLast = value; }
        }

        bool AnyPreBoundItems
        {
            get
            {
                Debug.Assert((_preBoundBlockFirst == null) == (_preBoundBlockLast == null));
                return _preBoundBlockFirst != null;
            }
        }

        #endregion

        #region Construction

        public GeneratorBinder(AquilaCompilation compilation,
            LocalsTable locals, SourceMethodSymbol method, object self)
            : base(compilation, method.Syntax.SyntaxTree, locals, method, self)
        {
            Debug.Assert(Method != null);

            // TODO: move this to SourceMethodSymbol ctor?
            Method.Flags |= MethodFlags.IsGenerator;
        }

        public override void SetupBuilder(Func<BoundBlock> newBlockFunc)
        {
            Debug.Assert(newBlockFunc != null);

            base.SetupBuilder(newBlockFunc);

            //
            _newBlockFunc = newBlockFunc;
        }

        #endregion

        #region GeneralOverrides

        public override void WithTryScopes(IEnumerable<TryCatchEdge> tryScopes)
        {
            _tryScopes = tryScopes;
        }

        public override BoundItemsBag<BoundExpression> BindWholeExpression(Expression expr, BoundAccess access)
        {
            Debug.Assert(!AnyPreBoundItems);

            var boundItem = BindExpression(expr, access);
            var boundBag = new BoundItemsBag<BoundExpression>(boundItem, _preBoundBlockFirst, _preBoundBlockLast);

            ClearPreBoundBlocks();

            return boundBag;
        }

        public override BoundItemsBag<BoundStatement> BindWholeStatement(Statement stmt)
        {
            Debug.Assert(!AnyPreBoundItems);

            var boundItem = BindStatement(stmt);
            var boundBag = new BoundItemsBag<BoundStatement>(boundItem, _preBoundBlockFirst, _preBoundBlockLast);

            ClearPreBoundBlocks();
            return boundBag;
        }

        protected override BoundExpression BindExpression(Expression expr, BoundAccess access)
        {
            var _underYieldLikeExLevelOnEnter = _underYieldLikeExLevel;

            // can't use only AST to determine whether we're under yield<>root route 
            //  -> there're expressions (such as foreach variable) outside in terms of semantics tree
            //  -> for those we don't want to do any moving (can actually be a problem for those)

            // update _underYieldLikeExLevel
            if (_underYieldLikeExLevel >= 0)
            {
                _underYieldLikeExLevel++;
            }

            if (_yieldsToStatementRootPath.Contains(expr))
            {
                _underYieldLikeExLevel = 0;
            }

            var boundExpr = base.BindExpression(expr, access);

            // move expressions on and directly under yieldLikeEx<>root path
            if (_underYieldLikeExLevel == 1 || _underYieldLikeExLevel == 0)
            {
                boundExpr = MakeTmpCopyAndPrependAssigment(boundExpr, access);
            }

            _underYieldLikeExLevel = _underYieldLikeExLevelOnEnter;
            return boundExpr;
        }

        protected override BoundStatement BindStatement(Statement stmt)
        {
            var _underYieldLikeExLevelOnEnter = _underYieldLikeExLevel;

            // update _underYieldLikeExLevel
            if (_underYieldLikeExLevel >= 0)
            {
                _underYieldLikeExLevel++;
            }

            if (_yieldsToStatementRootPath.Contains(stmt))
            {
                _underYieldLikeExLevel = 0;
            }

            var boundStatement = base.BindStatement(stmt);

            _underYieldLikeExLevel = _underYieldLikeExLevelOnEnter;
            return boundStatement;
        }

        #endregion

        #region SpecificExOverrides

        protected override BoundExpression BindBinaryEx(BinaryEx expr)
        {
            // if not a short-circuit operator -> use normal logic for binding
            if (!(expr.Operation == Operations.And || expr.Operation == Operations.Or ||
                  expr.Operation == Operations.Coalesce))
            {
                return base.BindBinaryEx(expr);
            }

            // create/get a source block defensively before potential rightExprBag.PreBoundBlocks (it needs to have a smaller Ordinal)
            var currBlock = CurrentPreBoundBlock;

            // left operand is always evaluated -> handle normally
            var leftExpr = BindExpression(expr.Left, BoundAccess.Read);

            // right operand & its pre-bound statements might not be evaluated due to short-circuit evaluation
            // get all of the pre-bound statements and convert them to statemenets conditioned by short-cirtuit-eval logic
            var rightExprBag = BindExpressionWithSeparatePreBoundStatements(expr.Right, BoundAccess.Read);

            if (!rightExprBag.IsOnlyBoundElement)
            {
                // make a defensive copy if multiple evaluations could be a problem for left expr (which serves as the condition)
                // no need to worry about order of execution: right bag contains preBoundStatements  
                // .. -> we are on yield<>root path -> expression to the left are already prepended
                leftExpr = MakeTmpCopyAndPrependAssigment(leftExpr, BoundAccess.Read);

                // create a condition expr. that is true only when right operand would have to be evaluated
                BoundExpression condition = null;
                switch (expr.Operation)
                {
                    case Operations.And:
                        condition = leftExpr; // left is true
                        break;
                    case Operations.Or:
                        condition = new BoundUnaryEx(leftExpr, Operations.LogicNegation, null); // left is false
                        break;
                    case Operations.Coalesce:
                        if (leftExpr is BoundReferenceEx leftRef) // left is not set or null
                        {
                            condition = new BoundUnaryEx(
                                new BoundIsSetEx(leftRef),
                                Operations.LogicNegation, null
                            );
                        }
                        else
                        {
                            // // Template: "is_null( LValue )"
                            // condition = new BoundGlobalFunctionCall(
                            //     NameUtils.SpecialNames.is_null, null,
                            //     ImmutableArray.Create(BoundArgument.Create(leftExpr)));
                        }

                        break;
                    default:
                        throw ExceptionUtilities.Unreachable;
                }

                // create a conditional edge and set the last (current) pre-bound block to the conditional edge's end block
                CurrentPreBoundBlock = CreateConditionalEdge(currBlock, condition, rightExprBag.PreBoundBlockFirst,
                    rightExprBag.PreBoundBlockLast, null, null);
            }

            return new BoundBinaryEx(
                leftExpr,
                rightExprBag.BoundElement,
                expr.Operation, null);
        }

        // protected override BoundExpression BindConditionalEx(ConditionalEx expr, BoundAccess access)
        // {
        //     var hastruebranch = (expr.TrueExpr != null);
        //
        //     var condExpr = BindExpression(expr.CondExpr, hastruebranch ? BoundAccess.Read : access.WithRead());
        //
        //     // create/get a source block defensively before potential true/falseExprBag.PreBoundBlocks (it needs to have a smaller Ordinal)
        //     var currBlock = CurrentPreBoundBlock;
        //
        //     // get expressions and their pre-bound elements for both branches
        //     var trueExprBag = BindExpressionWithSeparatePreBoundStatements(expr.TrueExpr, access);
        //     var falseExprBag = BindExpressionWithSeparatePreBoundStatements(expr.FalseExpr, access);
        //
        //     // if at least branch has any pre-bound statements we need to condition them
        //     if (!trueExprBag.IsOnlyBoundElement || !falseExprBag.IsOnlyBoundElement)
        //     {
        //         // make a defensive copy of condition, would be evaluated twice otherwise (conditioned prebound blocks and original position)
        //         // no need to worry about order of execution: either true or false branch contains preBoundStatements  
        //         // .. -> we are on yield<>root path -> expression to the left are already prepended
        //         condExpr = MakeTmpCopyAndPrependAssigment(condExpr, BoundAccess.Read);
        //
        //         // create a conditional edge and set the last (current) pre-bound block to the conditional edge's end block
        //         CurrentPreBoundBlock = CreateConditionalEdge(currBlock, condExpr,
        //             trueExprBag.PreBoundBlockFirst, trueExprBag.PreBoundBlockLast,
        //             falseExprBag.PreBoundBlockFirst, falseExprBag.PreBoundBlockLast);
        //     }
        //
        //     return new BoundConditionalEx(
        //         condExpr,
        //         trueExprBag.BoundElement,
        //         falseExprBag.BoundElement);
        // }

        // protected override BoundYieldEx BindYieldEx(YieldEx expr, BoundAccess access)
        // {
        //     // Reference: https://github.com/dotnet/roslyn/blob/05d923831e1bc2a88918a2073fba25ab060dda0c/src/Compilers/CSharp/Portable/Binder/Binder_Statements.cs#L194
        //
        //     // TODO: Throw error when trying to iterate a non-reference generator by reference 
        //     var valueaccess = _locals.Method.SyntaxSignature.AliasReturn
        //         ? BoundAccess.ReadRef
        //         : BoundAccess.Read;
        //
        //     // bind value and key expressions
        //     var boundValueExpr = (expr.ValueExpr != null) ? BindExpression(expr.ValueExpr, valueaccess) : null;
        //     var boundKeyExpr = (expr.KeyExpr != null) ? BindExpression(expr.KeyExpr) : null;
        //
        //     // bind yield statement (represents return & continuation)
        //     CurrentPreBoundBlock.Add(NewYieldStatement(boundValueExpr, boundKeyExpr, syntax: expr));
        //
        //     // return BoundYieldEx representing a reference to a value sent to the generator
        //     return new BoundYieldEx();
        // }

        // protected override BoundExpression BindYieldFromEx(YieldFromEx expr, BoundAccess access)
        // {
        //     var aliasedValues = _locals.Method.SyntaxSignature.AliasReturn;
        //     var tmpVar = MakeTmpCopyAndPrependAssigment(BindExpression(expr.ValueExpr), BoundAccess.Read);
        //
        //     /* Template:
        //      * foreach (<tmp> => <key> as <value>) {
        //      *     yield <key> => <value>;
        //      * }
        //      * return <tmp>.getReturn()
        //      */
        //
        //     string valueVarName = GetTempVariableName();
        //     string keyVarName = valueVarName + "k";
        //
        //     var move = NewBlock();
        //     var body = NewBlock();
        //     var end = NewBlock();
        //
        //     var enumereeEdge = new ForeachEnumereeEdge(CurrentPreBoundBlock, move, tmpVar, aliasedValues);
        //
        //     // MoveNext()
        //     var moveEdge = new ForeachMoveNextEdge(
        //         move, body, end, enumereeEdge,
        //         keyVar: new BoundTemporalVariableRef(keyVarName).WithAccess(BoundAccess.Write),
        //         valueVar: new BoundTemporalVariableRef(valueVarName).WithAccess(aliasedValues
        //             ? BoundAccess.Write.WithWriteRef(0)
        //             : BoundAccess.Write),
        //         moveSpan: default(Microsoft.CodeAnalysis.Text.TextSpan));
        //
        //     // body:
        //     // Template: yield key => value
        //     body.Add(
        //         NewYieldStatement(
        //             valueExpression: new BoundTemporalVariableRef(valueVarName).WithAccess(aliasedValues
        //                 ? BoundAccess.ReadRef
        //                 : BoundAccess.Read),
        //             keyExpression: new BoundTemporalVariableRef(keyVarName).WithAccess(BoundAccess.Read),
        //             isYieldFrom: true));
        //
        //     // goto move
        //     new SimpleEdge(body, move);
        //
        //     // end:
        //     CurrentPreBoundBlock = end;
        //
        //     // GET_RETURN( tmp as Generator )
        //     return new BoundYieldFromEx(tmpVar);
        // }

        #endregion

        #region Helpers

        public struct BoundSynthesizedVariableInfo
        {
            public BoundReferenceEx BoundExpr;
            public BoundAssignEx Assignment;
        }

        private void ClearPreBoundBlocks()
        {
            _preBoundBlockFirst = null;
            _preBoundBlockLast = null;
        }

        /// <summary>
        /// Assigns an expression to a temp variable, puts the assigment to <c>_preCurrentlyBinded</c>, and returns reference to the temp variable.
        /// </summary>
        private BoundExpression MakeTmpCopyAndPrependAssigment(BoundExpression boundExpr, BoundAccess access)
        {
            // no need to do anything if the expression is constant because neither multiple evaluation nor different order of eval is a problem
            if (boundExpr.IsConstant())
            {
                return boundExpr;
            }

            var assignVarTouple = CreateAndAssignSynthesizedVariable(boundExpr, access, GetTempVariableName());

            CurrentPreBoundBlock.Add(new BoundExpressionStmt(assignVarTouple.Assignment)); // assigment
            return assignVarTouple.BoundExpr; // temp variable ref
        }

        internal static BoundSynthesizedVariableInfo CreateAndAssignSynthesizedVariable(BoundExpression expr,
            BoundAccess access, string name)
        {
            // determine whether the synthesized variable should be by ref (for readRef and writes)
            var refAccess = (access.IsReadRef || access.IsWrite);

            // bind assigment target variable with appropriate access
            var targetVariable = new BoundTemporalVariableRef(name, null)
                .WithAccess(refAccess ? BoundAccess.Write.WithWriteRef() : BoundAccess.Write);

            // set appropriate access of the original value expression
            var valueBeingMoved =
                (refAccess) ? expr.WithAccess(BoundAccess.ReadRef) : expr.WithAccess(BoundAccess.Read);

            // bind assigment and reference to the created synthesized variable
            var assigment = new BoundAssignEx(targetVariable, valueBeingMoved, null);
            var boundExpr = new BoundTemporalVariableRef(name, null).WithAccess(access);

            return new BoundSynthesizedVariableInfo() { BoundExpr = boundExpr, Assignment = assigment };
        }

        /// <summary>
        /// Creates a conditional edge and returns its endBlock.
        /// </summary>
        private BoundBlock CreateConditionalEdge(BoundBlock sourceBlock, BoundExpression condExpr,
            BoundBlock trueBlockStart, BoundBlock trueBlockEnd, BoundBlock falseBlockStart, BoundBlock falseBlockEnd)
        {
            Debug.Assert(trueBlockStart != null || falseBlockStart != null);
            Debug.Assert(trueBlockStart == null ^ trueBlockEnd != null);
            Debug.Assert(falseBlockStart == null ^ falseBlockEnd != null);

            // if only false branch is non-empty flip the condition and conditioned blocks so that true is non-empty
            if (trueBlockStart == null)
            {
                condExpr = new BoundUnaryEx(condExpr, Operations.LogicNegation, null);
                trueBlockStart = falseBlockStart;
                trueBlockEnd = falseBlockEnd;
                falseBlockStart = null;
                falseBlockEnd = null;
            }

            var endBlock = NewBlock();
            falseBlockStart ??= endBlock;


            sourceBlock.NextCondition(trueBlockStart, falseBlockStart, condExpr);
            trueBlockEnd.NextSimple(endBlock);

            if (falseBlockStart != endBlock)
            {
                falseBlockEnd.NextSimple(endBlock);
            }

            return endBlock;
        }

        private BoundItemsBag<BoundExpression> BindExpressionWithSeparatePreBoundStatements(Expression expr,
            BoundAccess access)
        {
            if (expr == null)
            {
                return BoundItemsBag<BoundExpression>.Empty;
            }

            // save original preBoundBlocks
            var originalPreBoundFirst = _preBoundBlockFirst;
            var originalPreBoundLast = _preBoundBlockLast;

            ClearPreBoundBlocks(); // clean state
            var currExprBag = BindWholeExpression(expr, access);

            // restore original preBoundBlocks
            _preBoundBlockFirst = originalPreBoundFirst;
            _preBoundBlockLast = originalPreBoundLast;

            return currExprBag;
        }

        #endregion
    }
}