using System;
using System.Collections.Generic;
using System.Text;
using Aquila.CodeAnalysis.Utilities;

namespace Aquila.CodeAnalysis.Semantics.Graph
{
    /// <summary>
    /// Base visitor for control flow graphs.
    /// </summary>
    /// <typeparam name="TResult">Return type of all the Visit operations, use <see cref="VoidStruct"/> if none.</typeparam>
    public abstract class GraphVisitor<TResult> : AquilaOperationVisitor<TResult>
    {
        /// <summary>Visits given block.</summary>
        protected TResult Accept(BoundBlock x) => (x != null) ? x.Accept(this) : default;

        /// <summary>Visits given edge.</summary>
        protected TResult Accept(Edge x) => (x != null) ? x.Accept(this) : default;

        #region ControlFlowGraph

        public virtual TResult VisitCFG(ControlFlowGraph x) => default;

        #endregion

        #region Graph.Block

        protected virtual TResult DefaultVisitBlock(BoundBlock x) => default;

        public virtual TResult VisitCFGBlock(BoundBlock x) => DefaultVisitBlock(x);

        public virtual TResult VisitCFGStartBlock(StartBlock x) => VisitCFGBlock(x);

        public virtual TResult VisitCFGExitBlock(ExitBlock x) => DefaultVisitBlock(x);

        public virtual TResult VisitCFGCatchBlock(CatchBlock x) => DefaultVisitBlock(x);

        public virtual TResult VisitCFGCaseBlock(MatchArmBlock x) => DefaultVisitBlock(x);

        #endregion

        #region Graph.Edge

        protected virtual TResult DefaultVisitEdge(Graph.Edge x) => default;

        public virtual TResult VisitCFGSimpleEdge(Graph.SimpleEdge x) => DefaultVisitEdge(x);

        public virtual TResult VisitCFGLeaveEdge(Graph.LeaveEdge x) => VisitCFGSimpleEdge(x);

        public virtual TResult VisitCFGConditionalEdge(Graph.ConditionalEdge x) => DefaultVisitEdge(x);

        public virtual TResult VisitCFGTryCatchEdge(Graph.TryCatchEdge x) => DefaultVisitEdge(x);

        public virtual TResult VisitCFGForeachEnumereeEdge(Graph.ForeachEnumereeEdge x) => DefaultVisitEdge(x);

        public virtual TResult VisitCFGForeachEdge(Graph.ForeachEdge x) => DefaultVisitEdge(x);

        public virtual TResult VisitCFGForeachMoveNextEdge(Graph.ForeachMoveNextEdge x) => DefaultVisitEdge(x);

        public virtual TResult VisitCFGSwitchEdge(Graph.MatchEdge x) => DefaultVisitEdge(x);

        public virtual TResult VisitBuckStopEdge(Graph.BuckStopEdge x) => DefaultVisitEdge(x);

        #endregion
    }
}