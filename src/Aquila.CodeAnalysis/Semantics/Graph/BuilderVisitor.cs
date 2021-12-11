using Aquila.CodeAnalysis.Symbols;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Aquila.CodeAnalysis.Errors;
using Aquila.CodeAnalysis.Syntax;
using Aquila.Syntax;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Ast.Expressions;
using Aquila.Syntax.Syntax;
using Aquila.Syntax.Text;
using Microsoft.CodeAnalysis.Text;

namespace Aquila.CodeAnalysis.Semantics.Graph
{
    /// <summary>
    /// Visitor implementation that constructs the graph.
    /// </summary>
    internal sealed class BuilderVisitor : AquilaSyntaxWalker
    {
        readonly Binder _binder;

        private BoundBlock _current;

        private Dictionary<string, ControlFlowGraph.LabelBlockState> _labels;
        private List<BreakTargetScope> _breakTargets;
        private Stack<TryCatchEdge> _tryTargets;
        private Stack<LocalScopeInfo> _scopes = new Stack<LocalScopeInfo>(1);
        private int _index = 0;

        /// <summary>Counts visited "return" statements.</summary>
        private int _returnCounter = 0;

        /// <summary>
        /// Gets enumeration of unconditional declarations.
        /// </summary>
        public IEnumerable<BoundStatement> Declarations => _declarations ?? Enumerable.Empty<BoundStatement>();

        public List<BoundStatement> _declarations;

        public BoundBlock Start { get; private set; }

        public BoundBlock Exit { get; private set; }
        //public BoundBlock Exception { get; private set; }

        /// <summary>
        /// Gets labels defined within the method.
        /// </summary>
        public ImmutableArray<ControlFlowGraph.LabelBlockState> Labels
        {
            get
            {
                return (_labels != null)
                    ? _labels.Values.ToImmutableArray()
                    : ImmutableArray<ControlFlowGraph.LabelBlockState>.Empty;
            }
        }

        /// <summary>
        /// Blocks we know nothing is pointing to (right after jump, throw, etc.).
        /// </summary>
        public ImmutableArray<BoundBlock> DeadBlocks
        {
            get { return _deadBlocks.ToImmutableArray(); }
        }

        private readonly List<BoundBlock>
            _deadBlocks = new List<BoundBlock>();

        #region LocalScope

        private enum LocalScope
        {
            Code,
            Try,
            Catch,
            Finally,
        }

        private class LocalScopeInfo
        {
            public BoundBlock FirstBlock { get; }
            public LocalScope Scope { get; }

            public LocalScopeInfo(BoundBlock firstBlock, LocalScope scope)
            {
                this.FirstBlock = firstBlock;
                this.Scope = scope;
            }
        }

        private void OpenScope(BoundBlock block, LocalScope scope = LocalScope.Code)
        {
            _scopes.Push(new LocalScopeInfo(block, scope));
        }

        private void CloseScope()
        {
            if (_scopes.Count == 0)
                throw new InvalidOperationException();

            _scopes.Pop(); // .FirstBlock.ScopeTo = _index;
        }

        #endregion

        #region BreakTargetScope

        /// <summary>
        /// Represents break scope.
        /// </summary>
        private struct BreakTargetScope
        {
            public readonly BoundBlock
                BreakTarget;

            public readonly BoundBlock
                ContinueTarget;

            public BreakTargetScope(BoundBlock breakBlock, BoundBlock continueBlock)
            {
                BreakTarget = breakBlock;
                ContinueTarget = continueBlock;
            }
        }

        private BreakTargetScope GetBreakScope(int level)
        {
            if (level < 1) level = 1;
            if (_breakTargets == null || _breakTargets.Count < level)
                return default(BreakTargetScope);

            return _breakTargets[_breakTargets.Count - level];
        }

        private void OpenBreakScope(BoundBlock breakBlock, BoundBlock continueBlock)
        {
            if (_breakTargets == null) _breakTargets = new List<BreakTargetScope>(1);
            _breakTargets.Add(new BreakTargetScope(breakBlock, continueBlock));
        }

        private void CloseBreakScope()
        {
            Debug.Assert(_breakTargets != null && _breakTargets.Count != 0);
            _breakTargets.RemoveAt(_breakTargets.Count - 1);
        }

        #endregion

        #region TryTargetScope

        private TryCatchEdge CurrentTryScope
        {
            get
            {
                return (_tryTargets != null && _tryTargets.Count != 0)
                    ? _tryTargets.Peek()
                    : null;
            }
        }

        private void OpenTryScope(TryCatchEdge edge)
        {
            if (_tryTargets == null)
            {
                //_binder.WithTryScopes(_tryTargets = new Stack<TryCatchEdge>());
            }

            _tryTargets.Push(edge);
        }

        private void CloseTryScope()
        {
            Debug.Assert(_tryTargets != null && _tryTargets.Count != 0);
            _tryTargets.Pop();
        }

        #endregion

        #region Construction

        private BuilderVisitor(IList<StmtSyntax> statements, Binder binder)
        {
            Contract.ThrowIfNull(statements);
            Contract.ThrowIfNull(binder);

            _binder = binder;

            this.Start = WithNewOrdinal(new StartBlock());
            this.Exit = new ExitBlock();

            _current = WithOpenScope(this.Start);

            statements.ForEach(this.Visit);
            FinalizeMethod();
            _current = Connect(_current, this.Exit);

            //
            WithNewOrdinal(this.Exit);
            CloseScope();

            //
            Debug.Assert(_scopes.Count == 0);
            Debug.Assert(_tryTargets == null || _tryTargets.Count == 0);
            Debug.Assert(_breakTargets == null || _breakTargets.Count == 0);
        }

        public static BuilderVisitor Build(IList<StmtSyntax> statements, Binder binder)
        {
            return new BuilderVisitor(statements, binder);
        }

        #endregion

        #region Helper Methods

        private BoundBlock GetExceptionBlock()
        {
            //if (this.Exception == null)
            //    this.Exception = new ExitBlock();
            //return this.Exception;
            return this.Exit;
        }

        private void Add(StmtSyntax stmt)
        {
            Add(_binder.BindStatement(stmt));
        }

        private void Add(BoundItemsBag<BoundStatement> stmtBag)
        {
            ConnectBoundItemsBagBlocksToCurrentBlock(stmtBag);
            _current.Add(stmtBag.BoundElement);
        }

        private void ConnectBoundItemsBagBlocksToCurrentBlock<T>(BoundItemsBag<T> bag) where T : class, IAquilaOperation
        {
            _current = ConnectBoundItemsBagBlocks(bag, _current);
        }

        private BoundBlock ConnectBoundItemsBagBlocks<T>(BoundItemsBag<T> bag, BoundBlock block)
            where T : class, IAquilaOperation
        {
            if (bag.IsOnlyBoundElement)
            {
                return block;
            }

            Connect(block, bag.PreBoundBlockFirst);
            return bag.PreBoundBlockLast;
        }

        private void FinalizeMethod()
        {
            if (!_deadBlocks.Contains(_current))
            {
                AddFinalReturn();
            }
        }

        private void AddFinalReturn()
        {
            BoundExpression expression;

            if (_binder.Method.IsGlobalScope)
            {
                // global code returns 1 by default if no other value is specified
                expression =
                    new BoundLiteral(1, _binder.DeclaringCompilation.CoreTypes.Int32.Symbol).WithAccess(
                        BoundAccess.Read);
            }
            else
            {
                // function returns NULL by default (void?)
                expression = null; // void
            }

            // return <expression>;
            _current.Add(new BoundReturnStmt(expression));
        }

        private BoundBlock NewBlock()
        {
            return WithNewOrdinal(new BoundBlock());
        }

        /// <summary>
        /// Creates block we know nothing is pointing to.
        /// Such block will be analysed later whether it is empty or whether it contains some statements (which will be reported as unreachable).
        /// </summary>
        private BoundBlock NewDeadBlock()
        {
            var block = new BoundBlock();
            block.Ordinal = -1; // unreachable
            _deadBlocks.Add(block);
            return block;
        }

        private BoundBlock Connect(BoundBlock source, BoundBlock ifTarget, BoundBlock elseTarget,
            ExprSyntax condition, bool isloop = false)
        {
            if (condition != null)
            {
                // bind condition expression & connect pre-condition blocks to source
                var boundConditionBag = _binder.BindExpression(condition, BoundAccess.Read);
                source.NextCondition(ifTarget, elseTarget, boundConditionBag, isloop);
                return ifTarget;
            }
            else
            {
                // jump to ifTarget if there is no condition (always true) // e.g. for (;;) { [ifTarget] }
                return Connect(source, ifTarget);
            }
        }

        private BoundBlock Connect(BoundBlock source, BoundBlock target)
        {
            source.NextSimple(target);
            return target;
        }

        private BoundBlock Leave(BoundBlock source, BoundBlock target)
        {
            source.NextLeave(target);
            return target;
        }

        private ControlFlowGraph.LabelBlockState GetLabelBlock(string label)
        {
            if (_labels == null)
                _labels =
                    new Dictionary<string, ControlFlowGraph.LabelBlockState>(StringComparer
                        .Ordinal); // goto is case sensitive

            ControlFlowGraph.LabelBlockState result;
            if (!_labels.TryGetValue(label, out result))
            {
                _labels[label] = result = new ControlFlowGraph.LabelBlockState()
                {
                    TargetBlock = NewBlock(),
                    LabelSpan = default,
                    Label = label,
                    Flags = ControlFlowGraph.LabelBlockFlags.None,
                };
            }

            return result;
        }

        /// <summary>
        /// Gets new block index.
        /// </summary>
        private int NewOrdinal() => _index++;

        private T WithNewOrdinal<T>(T block) where T : BoundBlock
        {
            block.Ordinal = NewOrdinal();
            return block;
        }

        private T WithOpenScope<T>(T block, LocalScope scope = LocalScope.Code) where T : BoundBlock
        {
            OpenScope(block, scope);
            return block;
        }

        #endregion

        #region Declaration Statements

        void AddUnconditionalDeclaration(BoundStatement decl)
        {
            if (_declarations == null)
            {
                _declarations = new List<BoundStatement>();
            }

            _declarations.Add(decl);
        }


        public override void VisitMethodDecl(MethodDecl x)
        {
            // ignored
        }

        #endregion

        #region Flow-Thru Statements

        // public override void VisitEmptyStmt(EmptyStmt x)
        // {
        //     // ignored
        // }
        //
        // public override void VisitEchoStmt(EchoStmt x)
        // {
        //     Add(x);
        // }

        public override void VisitBlockStmt(BlockStmt x)
        {
            Add(_binder.BindEmptyStmt(new TextSpan(x.Span.Start, 1))); // {

            base.VisitBlockStmt(x); // visit nested statements

            Add(_binder.BindEmptyStmt(new TextSpan(x.Span.End - 1, 1))); // } // TODO: endif; etc.
        }

        // public override void VisitDeclareStmt(DeclareStmt x)
        // {
        //     Add(x);
        //
        //     base.VisitDeclareStmt(x); // visit inner statement, if present
        // }

        // public override void VisitGlobalCode(GlobalCode x)
        // {
        //     throw new InvalidOperationException();
        // }
        //
        // public override void VisitGlobalStmt(GlobalStmt x)
        // {
        //     Add(x);
        // }
        //
        // public override void VisitStaticStmt(StaticStmt x)
        // {
        //     Add(x);
        // }

        public override void VisitExpressionStmt(ExpressionStmt x)
        {
            Add(x);

            // if (x.Expression is ExitEx ee)
            // {
            //     // NOTE: Added by VisitExpressionStmt already
            //     // NOTE: similar to ThrowEx but unhandleable
            //
            //     // connect to Exception block
            //     Connect(_current, this.GetExceptionBlock());
            //     _current = NewDeadBlock();  // unreachable
            // }
            // else 
            if (x.Expression is ThrowEx te)
            {
                //var tryedge = GetTryTarget();
                //if (tryedge != null)
                //{
                //    // find handling catch block
                //    QualifiedName qname;
                //    var newex = x.Expression as NewEx;
                //    if (newex != null && newex.ClassNameRef is DirectTypeRef)
                //    {
                //        qname = ((DirectTypeRef)newex.ClassNameRef).ClassName;
                //    }
                //    else
                //    {
                //        qname = new QualifiedName(Name.EmptyBaseName);
                //    }

                //    CatchBlock handlingCatch = tryedge.HandlingCatch(qname);
                //    if (handlingCatch != null)
                //    {
                //        // throw jumps to a catch item in runtime
                //    }
                //}

                // connect to Exception block
                Connect(_current, this.GetExceptionBlock());
                _current = NewDeadBlock(); // unreachable
            }
        }

        #endregion

        #region Conditional Statements

        // public override void VisitConditionalStmt(ConditionalStmt x)
        // {
        //     throw new InvalidOperationException();  // should be handled by IfStmt
        // }

        // public override void VisitForeachStmt(ForeachStmt x)
        // {
        //     // binds enumeree expression & connect pre-enumeree-expr blocks
        //     var boundEnumereeBag = _binder.BindWholeExpression(x.Enumeree, BoundAccess.Read);
        //     ConnectBoundItemsBagBlocksToCurrentBlock(boundEnumereeBag);
        //
        //     var end = NewBlock();
        //     var move = NewBlock();
        //     var body = NewBlock();
        //
        //     // _current -> move -> body -> move -> ...
        //
        //     // ForeachEnumereeEdge : SimpleEdge
        //     // x.Enumeree.GetEnumerator();
        //     var enumereeEdge = new ForeachEnumereeEdge(_current, move, boundEnumereeBag.BoundElement, x.ValueVariable.Alias);
        //
        //     // ContinueTarget:
        //     OpenBreakScope(end, move);
        //
        //     // bind reference expression for foreach key variable
        //     var keyVar = (x.KeyVariable != null)
        //         ? (BoundReferenceExpression)_binder.BindWholeExpression(x.KeyVariable.Variable, BoundAccess.Write).SingleBoundElement()
        //         : null;
        //
        //     // bind reference expression for foreach value variable 
        //     var valueVar = (BoundReferenceExpression)(_binder.BindWholeExpression(
        //             (Expression)x.ValueVariable.Variable ?? x.ValueVariable.List,
        //             x.ValueVariable.Alias ? BoundAccess.Write.WithWriteRef(FlowAnalysis.TypeRefMask.AnyType) : BoundAccess.Write)
        //         .SingleBoundElement());
        //
        //     // ForeachMoveNextEdge : ConditionalEdge
        //     var moveEdge = new ForeachMoveNextEdge(
        //         move, body, end, enumereeEdge,
        //         keyVar, valueVar,
        //         x.GetMoveNextSpan());
        //     // while (enumerator.MoveNext()) {
        //     //   var value = enumerator.Current.Value
        //     //   var key = enumerator.Current.Key
        //
        //     // Block
        //     //   { x.Body }
        //     _current = WithOpenScope(WithNewOrdinal(body));
        //     VisitElement(x.Body);
        //     CloseScope();
        //     //   goto ContinueTarget;
        //     Connect(_current, move);
        //
        //     // BreakTarget:
        //     CloseBreakScope();
        //
        //     //
        //     _current = WithNewOrdinal(end);
        // }

        private void BuildForLoop(VariableDecl decl, ExprSyntax condExpr, List<ExprSyntax> incrementors,
            StmtSyntax bodyStmt)
        {
            var end = NewBlock();

            bool hasIncrementors = incrementors != null;
            bool hasConditions = condExpr != null;

            // { initializer }
            if (decl != null)
                VisitVariableDecl(decl);

            var body = NewBlock();
            var cond = hasConditions ? NewBlock() : body;
            var action = hasIncrementors ? NewBlock() : cond;
            OpenBreakScope(end, action);

            // while (x.Codition) {
            _current = WithNewOrdinal(Connect(_current, cond));
            if (hasConditions)
            {
                _current = WithNewOrdinal(Connect(_current, body, end, condExpr, true));
            }
            else
            {
                _deadBlocks.Add(end);
            }

            //   { x.Body }
            OpenScope(_current);
            Visit(bodyStmt);
            //   { x.Action }
            if (hasIncrementors)
            {
                _current = WithNewOrdinal(Connect(_current, action));

                foreach (var incrementor in incrementors)
                {
                    this.Add(SyntaxFactory.ExpressionStmt(incrementor));
                }
            }

            CloseScope();

            // }
            Connect(_current, cond);

            //
            CloseBreakScope();

            //
            _current = WithNewOrdinal(end);
        }

        private void BuildDoLoop(ExprSyntax condExpr, StmtSyntax bodyStmt)
        {
            var end = NewBlock();

            var body = NewBlock();
            var cond = NewBlock();

            OpenBreakScope(end, cond);

            // do { ...
            _current = WithNewOrdinal(Connect(_current, body));

            // x.Body
            OpenScope(_current);
            Visit(bodyStmt);
            CloseScope();

            // } while ( COND )
            _current = WithNewOrdinal(Connect(_current, cond));
            _current = WithNewOrdinal(Connect(_current, body, end, condExpr, true));

            //
            CloseBreakScope();

            //
            _current = WithNewOrdinal(end);
        }

        public override void VisitForStmt(ForStmt x)
        {
            BuildForLoop(x.Declaration, x.Condition, x.Incrementors.OfType<ExprSyntax>().ToList(), x.Statement);
        }

        public override void VisitVariableDecl(VariableDecl arg)
        {
            BuildVariableDecl(SyntaxFactory.LocalDeclStmt(arg));
        }

        private void BuildVariableDecl(LocalDeclStmt varDecl)
        {
            Add(varDecl);

            // //Transform the variable declaration into ExprStmt(AssignEx)
            // foreach (var decl in varDecl.Declarators)
            // {
            //     var assignEx = new AssignEx(decl.Span, SyntaxKind.AssignmentExpression, Operations.AssignValue,
            //         decl.Initializer,
            //         new NameEx(decl.Initializer.Span, SyntaxKind.NameExpression, Operations.Empty, decl.Identifier));
            //
            //     var statement = new ExpressionStmt(assignEx.Span, SyntaxKind.ExpressionStatement, assignEx);
            //
            //     Add(statement);
            // }
        }

        public override void VisitReturnStmt(ReturnStmt x)
        {
            // if (x.Type == JumpStmt.Types.Return)
            // {
            _returnCounter++;

            //
            Add(x);
            Connect(_current, this.Exit);
            _current = NewDeadBlock(); // anything after these statements is unreachable
        }

        public override void VisitIfStmt(IfStmt x)
        {
            var end = NewBlock();

            var conditions = x.Condition;

            BoundBlock elseBlock = null;

            var cond = conditions;

            if (x.Else != null) // if (Condition) ...
            {
                elseBlock = end;
                _current = Connect(_current, NewBlock(), elseBlock, cond);
            }
            else // else ...
            {
                elseBlock = end; // last ConditionalStmt
                _current = Connect(_current, NewBlock(), elseBlock, cond);
            }

            OpenScope(_current);
            Visit(x.Statement);
            CloseScope();

            Connect(_current, end);
            _current = WithNewOrdinal(elseBlock);
        }

        // public override void VisitLabelStmt(LabelStmt x)
        // {
        //     var  
        //         label = GetLabelBlock(x.Name.Name.Value);
        //     if ((label.Flags & ControlFlowGraph.LabelBlockFlags.Defined) != 0)
        //     {
        //         label.Flags |= ControlFlowGraph.LabelBlockFlags.Redefined; // label was defined already
        //         return; // ignore label redefinition                
        //     }
        //
        //     label.Flags |= ControlFlowGraph.LabelBlockFlags.Defined; // label is defined
        //     label.LabelSpan = x.Name.Span;
        //
        //     _current = WithNewOrdinal(Connect(_current, label.TargetBlock));
        // }

        // public override void VisitSwitchStmt(SwitchStmt x)
        // {
        //     var items = x.SwitchItems;
        //     if (items == null || items.Length == 0)
        //         return;
        //
        //     // get bound item for switch value & connect potential pre-switch-value blocks
        //     var boundBagForSwitchValue = _binder.BindWholeExpression(x.SwitchValue, BoundAccess.Read);
        //     ConnectBoundItemsBagBlocksToCurrentBlock(boundBagForSwitchValue);
        //     var switchValue = boundBagForSwitchValue.BoundElement;
        //
        //     var end = NewBlock();
        //
        //     bool hasDefault = false;
        //     var cases = new List<CaseBlock>(items.Length);
        //     for (int i = 0; i < items.Length; i++)
        //     {
        //         cases.Add(NewBlock(items[i]));
        //         hasDefault |= (items[i] is DefaultItem);
        //     }
        //
        //     if (!hasDefault)
        //     {
        //         // create implicit default:
        //         cases.Add(NewBlock(new DefaultItem(x.Span, EmptyArray<Statement>.Instance)));
        //     }
        //
        //     // if switch value isn't a constant & there're case values with preBoundStatements 
        //     // -> the switch value might get evaluated multiple times (see SwitchEdge.Generate) -> preemptively evaluate and cache it
        //     if (!switchValue.IsConstant() && !cases.All(c => c.CaseValue.IsOnlyBoundElement))
        //     {
        //         var result = GeneratorSemanticsBinder.CreateAndAssignSynthesizedVariable(switchValue, BoundAccess.Read,
        //             $"<switchValueCacher>{x.Span}");
        //         switchValue = result.BoundExpr;
        //         _current.Add(new BoundExpressionStatement(result.Assignment));
        //     }
        //
        //     // SwitchEdge // Connects _current to cases
        //     var edge = new SwitchEdge(_current, switchValue, cases.ToImmutableArray(), end);
        //     _current = WithNewOrdinal(cases[0]);
        //
        //     OpenBreakScope(end, end); // NOTE: inside switch, Continue ~ Break
        //
        //     for (int i = 0; i < cases.Count; i++)
        //     {
        //         OpenScope(_current);
        //
        //         if (i < items.Length)
        //             items[i].Statements.ForEach(VisitElement); // any break will connect block to end
        //
        //         CloseScope();
        //
        //         _current = WithNewOrdinal(Connect(_current, (i == cases.Count - 1) ? end : cases[i + 1]));
        //     }
        //
        //     CloseBreakScope();
        //
        //     Debug.Assert(_current == end);
        // }

        public override void VisitThrowEx(ThrowEx x)
        {
            base.VisitThrowEx(x);
        }


        public override void VisitMatchEx(MatchEx arg)
        {
            var items = arg.Arms.OfType<MatchArm>().ToArray();
            if (!items.Any())
                return;

            // get bound item for switch value & connect potential pre-switch-value blocks
            var matchValue = _binder.BindExpression(arg.Expression, BoundAccess.Read);

            var end = NewBlock();

            bool hasDefault = false;
            var arms = new List<MatchArmBlock>(items.Count());

            for (int i = 0; i < items.Count(); i++)
            {
                var arm = new MatchArmBlock(_binder.BindExpression(items[i].PatternExpression, BoundAccess.Read));
                arms.Add(arm);

                hasDefault |= arm.IsDefault;
            }

            // if (!hasDefault)
            // {
            //     // create implicit default:
            //     cases.Add(NewBlock(new DefaultItem(x.Span, EmptyArray<Statement>.Instance)));
            // }

            // // if switch value isn't a constant & there're case values with preBoundStatements 
            // // -> the switch value might get evaluated multiple times (see SwitchEdge.Generate) -> preemptively evaluate and cache it
            // if (!matchValue.IsConstant() && !cases.All(c => c.CaseValue.IsOnlyBoundElement))
            // {
            //     var result = GeneratorSemanticsBinder.CreateAndAssignSynthesizedVariable(switchValue, BoundAccess.Read,
            //         $"<switchValueCacher>{x.Span}");
            //     switchValue = result.BoundExpr;
            //     _current.Add(new BoundExpressionStatement(result.Assignment));
            // }

            // SwitchEdge // Connects _current to cases
            var edge = new MatchEdge(matchValue, arms.ToImmutableArray(), end);
            _current = WithNewOrdinal(arms[0]);

            OpenBreakScope(end, end); // NOTE: inside switch, Continue ~ Break

            for (int i = 0; i < arms.Count; i++)
            {
                OpenScope(_current);

                if (i < items.Length)
                    Visit(items[i].ResultExpression); // any break will connect block to end

                CloseScope();

                _current = WithNewOrdinal(Connect(_current, (i == arms.Count - 1) ? end : arms[i + 1]));
            }

            CloseBreakScope();

            Debug.Assert(_current == end);
        }

        // public override void VisitTryStmt(TryStmt x)
// {
//     // try {
//     //   x.Body
//     // }
//     // catch (E1) { body }
//     // catch (E2) { body }
//     // finally { body }
//     // end
//
//     var end = NewBlock();
//     var body = NewBlock();
//
//     // init catch blocks and finally block
//     var catchBlocks = ImmutableArray<CatchBlock>.Empty;
//     if (x.Catches != null)
//     {
//         var catchBuilder = ImmutableArray.CreateBuilder<CatchBlock>(x.Catches.Length);
//         for (int i = 0; i < x.Catches.Length; i++)
//         {
//             catchBuilder.Add(NewBlock(x.Catches[i]));
//         }
//
//         catchBlocks = catchBuilder.MoveToImmutable();
//     }
//
//     BoundBlock finallyBlock = null;
//     if (x.FinallyItem != null)
//         finallyBlock = NewBlock();
//
//     // TryCatchEdge // Connects _current to body, catch blocks and finally
//     var edge = new TryCatchEdge(_current, body, catchBlocks, finallyBlock, end);
//
//     var oldstates0 = _binder.StatesCount;
//
//     // build try body
//     OpenTryScope(edge);
//     OpenScope(body, LocalScope.Try);
//     _current = WithNewOrdinal(body);
//     VisitElement(x.Body);
//     CloseScope();
//     CloseTryScope();
//     _current = Leave(_current, finallyBlock ?? end);
//
//     var oldstates1 = _binder.StatesCount;
//
//     // built catches
//     for (int i = 0; i < catchBlocks.Length; i++)
//     {
//         _current = WithOpenScope(WithNewOrdinal(catchBlocks[i]), LocalScope.Catch);
//         VisitElement(x.Catches[i].Body);
//         CloseScope();
//         _current = Leave(_current, finallyBlock ?? end);
//     }
//
//     // build finally
//     var oldReturnCount = _returnCounter;
//     if (finallyBlock != null)
//     {
//         _current = WithOpenScope(WithNewOrdinal(finallyBlock), LocalScope.Finally);
//         VisitElement(x.FinallyItem.Body);
//         CloseScope();
//         _current = Leave(_current, end);
//     }
//
//     //
//     if ((oldstates0 != oldstates1 && finallyBlock != null) || // yield in "try" with "finally" block
//         oldstates1 != _binder.StatesCount || // yield in "catch" or "finally"
//         oldReturnCount != _returnCounter) // return in "finally"
//     {
//         // catch or finally introduces new states to the state machine
//         // or there is "return" in finally block:
//
//         // catch/finally must not be handled by CLR
//         edge.EmitCatchFinallyOutsideScope = true;
//     }
//
//     // _current == end
//     _current.Ordinal = NewOrdinal();
// }

        // public override void VisitDoWhileStmt(DoWhileStmt x)
        // {
        //     Debug.Assert(x.Condition != null);
        //
        //     // do { BODY } while (COND)
        //     BuildDoLoop(x.Condition, x.Block);
        // }

        public override void VisitWhileStmt(WhileStmt x)
        {
            Debug.Assert(x.Condition != null);

            // while (COND) { BODY }
            BuildForLoop(null, x.Condition, null, x.Statement);
        }

        #endregion
    }
}