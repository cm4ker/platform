using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Syntax;
using Aquila.Syntax.Ast;
using Microsoft.CodeAnalysis;
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

        /// <summary>
        /// Blocks between we can invoke break operator
        /// </summary>
        /// <param name="breakBlock"></param>
        /// <param name="continueBlock"></param>
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
                _tryTargets = new Stack<TryCatchEdge>();
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

        private BuilderVisitor(IReadOnlyList<HtmlNodeSyntax> nodes, Binder binder)
        {
            Contract.ThrowIfNull(nodes);
            Contract.ThrowIfNull(binder);

            _binder = binder;

            this.Start = WithNewOrdinal(new StartBlock());
            this.Exit = new ExitBlock();

            _current = WithOpenScope(this.Start);

            nodes.ForEach(this.Visit);
            FinalizeMethod();
            _current = Connect(_current, this.Exit);

            WithNewOrdinal(this.Exit);
            CloseScope();

            Debug.Assert(_scopes.Count == 0);
            Debug.Assert(_tryTargets == null || _tryTargets.Count == 0);
            Debug.Assert(_breakTargets == null || _breakTargets.Count == 0);
        }

        public static BuilderVisitor Build(IList<StmtSyntax> statements, Binder binder)
        {
            return new BuilderVisitor(statements, binder);
        }

        public static BuilderVisitor Build(IReadOnlyList<HtmlNodeSyntax> nodes, Binder binder)
        {
            return new BuilderVisitor(nodes, binder);
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
                    new BoundLiteral(1, _binder.Compilation.CoreTypes.Int32.Symbol).WithAccess(
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

        // private CatchBlock/*!*/NewBlock(CatchClauseSyntax item)
        // {
        //     //return WithNewOrdinal(new CatchBlock(_binder.BindTypeRef(item.TargetType), _binder.BindCatchVariable(item)));
        // }

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
                return Connect(source, ifTarget, elseTarget, boundConditionBag, isloop);
            }
            else
            {
                // jump to ifTarget if there is no condition (always true) // e.g. for (;;) { [ifTarget] }
                return Connect(source, ifTarget);
            }
        }

        private BoundBlock Connect(BoundBlock source, BoundBlock ifTarget, BoundBlock elseTarget,
            BoundExpression condition, bool isloop = false)
        {
            if (condition != null)
            {
                source.NextCondition(ifTarget, elseTarget, condition, isloop);
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


        public override void VisitFuncDecl(FuncDecl node)
        {
            //ignored
        }

        public override void VisitMethodDecl(MethodDecl x)
        {
            // ignored
        }

        #endregion

        #region Flow-Thru Statements

        public override void VisitBlockStmt(BlockStmt x)
        {
            Add(_binder.BindEmptyStmt(new TextSpan(x.Span.Start, 1))); // {

            base.VisitBlockStmt(x); // visit nested statements

            Add(_binder.BindEmptyStmt(new TextSpan(x.Span.End - 1, 1))); // } // TODO: endif; etc.
        }

        public override void VisitExpressionStmt(ExpressionStmt x)
        {
            Add(x);

            if (x.Expression is ThrowEx te)
            {
                // connect to Exception block
                Connect(_current, this.GetExceptionBlock());
                _current = NewDeadBlock(); // unreachable
            }
        }

        #endregion

        #region Conditional Statements

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

        private void BuildForeachLoop(ForEachStmt x)
        {
            var bound = _binder.BindStatement(x);
            if (bound is BoundForEachStmt forEach)
            {
                var bi = forEach.BoundInfo;

                var assign = bi.EnumeratorAssignmentEx;
                Add(new BoundExpressionStmt(assign));

                var moveNextCondition = bi.MoveNextEx;

                var itemAssign = new BoundExpressionStmt(new BoundAssignEx(
                        forEach.Item.WithAccess(BoundAccess.ReadAndWrite),
                        new BoundPropertyRef(bi.CurrentMember, assign.Target).WithAccess(BoundAccess.ReadAndWrite))
                    .WithAccess(BoundAccess.ReadAndWrite));
                var foreachEndBlock = NewBlock();

                var body = NewBlock();
                var move = NewBlock();

                var endCycle = NewBlock();
                endCycle.WithEdge(new BuckStopEdge());

                var edge = new ForeachEdge(forEach, WithNewOrdinal(move), body, foreachEndBlock);
                _current.WithEdge(edge);

                OpenBreakScope(endCycle, move);

                _current = WithNewOrdinal(Connect(move, body, endCycle, moveNextCondition, true));
                _deadBlocks.Add(endCycle);

                OpenScope(_current);
                Add(itemAssign);
                Visit(x.Statement);

                CloseScope();

                Connect(_current, move);

                CloseBreakScope();

                WithNewOrdinal(endCycle);

                _current = WithNewOrdinal(foreachEndBlock);
            }
            else
            {
                //ignore this is error
            }
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
            BuildForLoop(x.Declaration, x.Condition, x.Incrementors.ToList(), x.Statement);
        }

        public override void VisitForEachStmt(ForEachStmt node)
        {
            BuildForeachLoop(node);
        }

        public override void VisitVariableDecl(VariableDecl arg)
        {
            Add(_binder.BindVarDecl(arg));
        }

        public override void VisitLocalDeclStmt(LocalDeclStmt node)
        {
            BuildVariableDecl(node);
        }

        private void BuildVariableDecl(LocalDeclStmt varDecl)
        {
            Add(varDecl);
        }

        public override void VisitReturnStmt(ReturnStmt x)
        {
            _returnCounter++;

            //
            Add(x);
            Connect(_current, this.Exit);
            _current = NewDeadBlock(); // anything after these statements is unreachable
        }

        public override void VisitIfStmt(IfStmt x)
        {
            var end = NewBlock();

            var condition = x.Condition;

            // if (Condition) ...
            BoundBlock elseBlock = (x.Else != null) ? NewBlock() : end;

            _current = Connect(_current, NewBlock(), elseBlock, condition);

            OpenScope(_current);
            Visit(x.Statement);
            CloseScope();

            //connect if brunch to the end
            Connect(_current, end);
            _current = WithNewOrdinal(elseBlock);

            if (x.Else != null) // else ...
            {
                OpenScope(_current);
                Visit(x.Else.Statement);
                CloseScope();

                Connect(_current, end);
                _current = WithNewOrdinal(end);
            }

            Debug.Assert(_current == end);
            WithNewOrdinal(_current);
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

        public override void VisitWhileStmt(WhileStmt x)
        {
            Debug.Assert(x.Condition != null);

            // while (COND) { BODY }
            BuildForLoop(null, x.Condition, null, x.Statement);
        }

        public override void VisitTryStmt(TryStmt x)
        {
            // try {
            //   x.Body
            // }
            // catch (E1) { body }
            // catch (E2) { body }
            // finally { body }
            // end

            var end = NewBlock();
            var body = NewBlock();

            // init catch blocks and finally block
            var catchBlocks = ImmutableArray<CatchBlock>.Empty;
            if (x.Catches != null)
            {
                var catchBuilder = ImmutableArray.CreateBuilder<CatchBlock>(x.Catches.Count);
                for (int i = 0; i < x.Catches.Count; i++)
                {
                    catchBuilder.Add(new CatchBlock(null, null));
                }

                catchBlocks = catchBuilder.MoveToImmutable();
            }

            BoundBlock finallyBlock = null;
            if (x.Finally != null)
                finallyBlock = NewBlock();

            var edge = new TryCatchEdge(body, catchBlocks, finallyBlock, end);
            _current.SetNextEdge(edge);

            //var oldstates0 = _binder.StatesCount;

            // build try body
            OpenTryScope(edge);
            OpenScope(body, LocalScope.Try);
            _current = WithNewOrdinal(body);
            Visit(x.Block);
            CloseScope();
            CloseTryScope();
            _current = Leave(_current, finallyBlock ?? end);

            //var oldstates1 = _binder.StatesCount;

            // built catches
            for (int i = 0; i < catchBlocks.Length; i++)
            {
                _current = WithOpenScope(WithNewOrdinal(catchBlocks[i]), LocalScope.Catch);
                Visit(x.Catches[i].Block);
                CloseScope();
                _current = Leave(_current, finallyBlock ?? end);
            }

            // build finally
            var oldReturnCount = _returnCounter;
            if (finallyBlock != null)
            {
                _current = WithOpenScope(WithNewOrdinal(finallyBlock), LocalScope.Finally);
                Visit(x.Finally.Block);
                CloseScope();
                _current = Leave(_current, end);
            }

            // _current == end
            _current.Ordinal = NewOrdinal();
        }


        public override void VisitBreakStmt(BreakStmt x)
        {
            var brk = GetBreakScope(1);
            var target = brk.BreakTarget;
            if (target != null)
            {
                Connect(_current, target);
                _current.NextEdge.AquilaSyntax = x;
            }
            else
            {
                _binder.Diagnostics.Add(_binder.Method, x, Errors.ErrorCode.ERR_NeedsLoopOrSwitch, "break");
                Connect(_current, this.GetExceptionBlock()); // unreachable, wouldn't compile
            }

            _current = NewDeadBlock(); // anything after these statements is unreachable
        }

        public override void VisitContinueStmt(ContinueStmt x)
        {
            var brk = GetBreakScope(1);
            var target = brk.ContinueTarget;
            if (target != null)
            {
                Connect(_current, target);
                _current.NextEdge.AquilaSyntax = x;
            }
            else
            {
                _binder.Diagnostics.Add(_binder.Method, x, Errors.ErrorCode.ERR_NeedsLoopOrSwitch, "continue");
                Connect(_current, this.GetExceptionBlock()); // unreachable, wouldn't compile
            }

            _current = NewDeadBlock(); // anything after these statements is unreachable
        }

        #endregion

        #region Html

        public override void VisitHtmlEmptyElement(HtmlEmptyElementSyntax node)
        {
            Add(new BoundHtmlOpenElementStmt(node.Name.TagName.Text));
            node.Attributes.ForEach(Visit);
            base.VisitHtmlEmptyElement(node);
            Add(new BoundHtmlCloseElementStmt());
        }
        
        public override void VisitHtmlElement(HtmlElementSyntax node)
        {
            Add(new BoundHtmlOpenElementStmt(node.StartTag.Name.TagName.Text));
            node.StartTag.Attributes.ForEach(Visit);
            node.Content.ForEach(Visit);
            Add(new BoundHtmlCloseElementStmt());
        }

        public override void VisitHtmlAttribute(HtmlAttributeSyntax node)
        {
            BoundExpression main = null;
            var stringType = _binder.Compilation.CoreTypes.String.Symbol;
            
            foreach (var attributeItem in node.Nodes)
            {
                BoundExpression expression = attributeItem switch
                {
                    HtmlTextSyntax t => new BoundLiteral(t.Text.Text, stringType),
                    HtmlExpressionSyntax ex => _binder.BindExpression(ex.Expression, BoundAccess.Read),
                    _ => throw new ArgumentOutOfRangeException()
                };

                main = main == null ? expression : new BoundBinaryEx(main, expression, Operations.Add, stringType);
            }
            
            Add(new BoundHtmlAddAttributeStmt(node.Name.TagName.Text, main));
        }

        #endregion
    }
}