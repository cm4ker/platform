﻿using System;
using System.Diagnostics;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Semantics.Graph;

namespace Aquila.CodeAnalysis.FlowAnalysis
{
    public abstract class AnalysisWalker<TState, TResult> : GraphWalker<TResult>
    {
        #region Nested enum: AnalysisFlags

        /// <summary>
        /// Analysis progress flags.
        /// </summary>
        protected enum AnalysisFlags
        {
            /// <summary>
            /// The analysis has been canceled since internal state has changed.
            /// </summary>
            IsCanceled = 1,
        }

        #endregion

        #region Fields

        /// <summary>
        /// Current flow state.
        /// </summary>
        internal TState State { get; private protected set; }

        /// <summary>
        /// Gets reference to current block.
        /// </summary>
        internal BoundBlock CurrentBlock { get; private set; }

        /// <summary>
        /// Gathered analysis progress.
        /// </summary>
        protected AnalysisFlags _flags;

        #endregion

        #region State and worklist handling

        protected abstract bool IsStateInitialized(TState state);

        protected abstract bool AreStatesEqual(TState a, TState b);

        protected abstract TState GetState(BoundBlock block);

        protected abstract void SetState(BoundBlock block, TState state);

        protected abstract TState CloneState(TState state);

        protected abstract TState MergeStates(TState a, TState b);

        protected abstract void EnqueueBlock(BoundBlock block);

        #endregion

        #region Helper Methods

        /// <summary>
        /// Debug assert the state is initialized therefore we are in the middle on a block analysis.
        /// </summary>
        void AssertState()
        {
            Debug.Assert(State != null);
        }

        /// <summary>
        /// Helper method that merges state with the target block and determines whether to continue by visiting the target block.
        /// </summary>
        /// <param name="edgeLabel">Label identifying incoming edge.</param>
        /// <param name="state">Locals state in which we are entering the target.</param>
        /// <param name="target">Target block.</param>
        /// <remarks>Only for traversing into blocks within the same method (same type context).</remarks>
        private void TraverseToBlock(object edgeLabel, TState state, BoundBlock target)
        {
            if (!IsStateInitialized(state))
                throw new ArgumentException(nameof(state)); // state should be already set by previous block

            var targetState = GetState(target);
            if (targetState != null)
            {
                // block was visited already,
                // merge and check whether state changed
                state = MergeStates(state, targetState); // merge states into new one

                if (AreStatesEqual(state, targetState) && !target.ForceRepeatedAnalysis)
                {
                    // state converged, we don't have to analyse the target block again
                    // unless it is specially needed (e.g. ExitBlock)
                    return;
                }
            }
            else
            {
                // block was not visited yet
                state = CloneState(state); // copy state into new one
            }

            // update target block state
            SetState(target, state);

            //
            EnqueueBlock(target);
        }

        /// <summary>
        /// Called to initialize <see cref="VisitCFGBlock"/> call.
        /// Sets <see cref="State"/> to known initial block state.
        /// </summary>
        protected virtual void VisitCFGBlockInit(BoundBlock x)
        {
            var state = GetState(x);

            if (!IsStateInitialized(state))
                throw new ArgumentException(nameof(x)); // state should be already set by previous edge

            State = CloneState(state); // TState for the statements in the block

            this.CurrentBlock = x;
        }

        /// <summary>
        /// Updates the expression access and visits it.
        /// </summary>
        /// <param name="x">The expression.</param>
        /// <param name="access">New access.</param>
        protected void Visit(BoundExpression x, BoundAccess access)
        {
            x.Access = access;
            Accept(x);
        }

        #endregion

        #region Short-Circuit Evaluation

        /// <summary>
        /// Visits condition used to branch execution to true or false branch.
        /// </summary>
        /// <returns>Value indicating whether branch was used.</returns>
        /// <remarks>
        /// Because of minimal evaluation there is different state for true and false branches,
        /// AND and OR operators have to take this into account.
        /// 
        /// Also some other constructs may have side-effect for known branch,
        /// eg. <c>($x instanceof X)</c> implies ($x is X) in True branch.
        /// </remarks>
        internal bool VisitCondition(BoundExpression condition, ConditionBranch branch)
        {
            Contract.ThrowIfNull(condition);

            if (branch != ConditionBranch.AnyResult)
            {
                if (condition is BoundBinaryEx)
                {
                    Visit((BoundBinaryEx) condition, branch);
                    return true;
                }

                if (condition is BoundUnaryEx unaryEx)
                {
                    Visit(unaryEx, branch);
                    return true;
                }
            }

            // no effect
            condition.Accept(this);
            return false;
        }

        public sealed override TResult VisitBinaryEx(BoundBinaryEx x)
        {
            Visit(x, ConditionBranch.Default);

            return default;
        }

        protected virtual void Visit(BoundBinaryEx x, ConditionBranch branch)
        {
            base.VisitBinaryEx(x);
        }

        public sealed override TResult VisitUnaryEx(BoundUnaryEx x)
        {
            Visit(x, ConditionBranch.Default);

            return default;
        }

        protected virtual void Visit(BoundUnaryEx x, ConditionBranch branch)
        {
            base.VisitUnaryEx(x);
        }

        #endregion

        #region Specific

        #endregion

        #region GraphVisitor Members

        public override TResult VisitCFG(ControlFlowGraph x)
        {
            Contract.ThrowIfNull(x);
            Debug.Assert(IsStateInitialized(GetState(x.Start)), "Start block has to have an initial state set.");

            EnqueueBlock(x.Start);

            return default;
        }

        public override TResult VisitCFGBlock(BoundBlock x)
        {
            VisitCFGBlockInit(x);
            DefaultVisitBlock(x); // modifies State, traverses to the edge

            return default;
        }

        public override TResult VisitCFGCaseBlock(MatchArmBlock x)
        {
            VisitCFGBlockInit(x);
            DefaultVisitBlock(x);

            return default;
        }

        public override TResult VisitCFGCatchBlock(CatchBlock x)
        {
            VisitCFGBlockInit(x);

            Accept(x.TypeRef);
            Accept(x.Variable);

            //
            DefaultVisitBlock(x);

            return default;
        }

        public override TResult VisitCFGSimpleEdge(SimpleEdge x)
        {
            TraverseToBlock(x, State, x.NextBlock);

            return default;
        }

        public override TResult VisitCFGConditionalEdge(ConditionalEdge x)
        {
            // build state for TrueBlock and FalseBlock properly, take minimal evaluation into account
            var state = State;

            // true branch
            State = CloneState(state);
            VisitCondition(x.Condition, ConditionBranch.ToTrue);
            TraverseToBlock(x, State, x.TrueTarget);

            // false branch
            State = CloneState(state);
            VisitCondition(x.Condition, ConditionBranch.ToFalse);
            TraverseToBlock(x, State, x.FalseTarget);

            return default;
        }

        public override TResult VisitCFGForeachEnumereeEdge(ForeachEnumereeEdge x)
        {
            Accept(x.Enumeree);
            VisitCFGSimpleEdge(x);

            return default;
        }

        public override TResult VisitCFGForeachMoveNextEdge(ForeachMoveNextEdge x)
        {
            var state = State;


            // Body branch
            State = CloneState(state);
            // set key variable and value variable at current state

            var valueVar = x.ValueVariable;
            var islistunpacking = valueVar is BoundListEx;

            // analyse Value
            Visit(valueVar,
                valueVar.Access.WithWrite());

            // analyse Key
            var keyVar = x.KeyVariable;
            if (keyVar != null)
            {
                Visit(keyVar, keyVar.Access.WithWrite());
            }

            TraverseToBlock(x, State, x.BodyBlock);

            // End branch
            State = CloneState(state);
            TraverseToBlock(x, State, x.NextBlock);

            return default;
        }

        public override TResult VisitCFGForeachEdge(ForeachEdge x)
        {
            TraverseToBlock(x, State, x.Body);
            TraverseToBlock(x, State, x.Move);
            TraverseToBlock(x, State, x.End);
            Accept(x.Condition);
            return default;
        }

        public override TResult VisitCFGSwitchEdge(MatchEdge x)
        {
            Accept(x.SwitchValue);

            var state = State;

            foreach (var c in x.MatchBlocks)
            {
                TraverseToBlock(x, state, c);
            }

            return default;
        }

        public override TResult VisitCFGTryCatchEdge(TryCatchEdge x)
        {
            var state = State;
            
            TraverseToBlock(x, state, x.BodyBlock);

            foreach (var c in x.CatchBlocks)
            {
                TraverseToBlock(x, state, c);
            }

            if (x.FinallyBlock != null)
            {
                TraverseToBlock(x, state, x.FinallyBlock);
            }

            return default;
        }

        #endregion
    }
}