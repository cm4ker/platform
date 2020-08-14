﻿using System;
using System.Collections.Generic;
using System.Text;
using Peachpie.CodeAnalysis.Utilities;

namespace Pchp.CodeAnalysis.Semantics.Graph
{
    /// <summary>
    /// Base visitor for control flow graphs.
    /// </summary>
    /// <typeparam name="TResult">Return type of all the Visit operations, use <see cref="VoidStruct"/> if none.</typeparam>
    public abstract class GraphVisitor<TResult> : PhpOperationVisitor<TResult>
    {
        /// <summary>Visits given block.</summary>
        protected TResult Accept(BoundBlock x) => (x != null) ? x.Accept(this) : default;

        /// <summary>Visits given edge.</summary>
        protected TResult Accept(Edge x) => (x != null) ? x.AcceptGraph(this) : default;

        #region ControlFlowGraph

        public virtual TResult VisitCFG(ControlFlowGraph x) => default;

        #endregion

        #region Graph.Block

        protected virtual TResult DefaultVisitBlock(BoundBlock x) => default;

        public virtual TResult VisitCFGBlock(BoundBlock x) => DefaultVisitBlock(x);

        public virtual TResult VisitCFGStartBlock(StartBlock x) => VisitCFGBlock(x);

        public virtual TResult VisitCFGExitBlock(ExitBlock x) => DefaultVisitBlock(x);

        public virtual TResult VisitCFGCatchBlock(CatchBlock x) => DefaultVisitBlock(x);

        public virtual TResult VisitCFGCaseBlock(CaseBlock x) => DefaultVisitBlock(x);

        #endregion

        #region Graph.Edge

        protected virtual TResult DefaultVisitEdge(Graph.Edge x) => default;

        public virtual TResult VisitCFGSimpleEdge(Graph.SimpleEdge x) => DefaultVisitEdge(x);

        public virtual TResult VisitCFGLeaveEdge(Graph.LeaveEdge x) => VisitCFGSimpleEdge(x);

        public virtual TResult VisitCFGConditionalEdge(Graph.ConditionalEdge x) => DefaultVisitEdge(x);

        public virtual TResult VisitCFGTryCatchEdge(Graph.TryCatchEdge x) => DefaultVisitEdge(x);

        public virtual TResult VisitCFGForeachEnumereeEdge(Graph.ForeachEnumereeEdge x) => DefaultVisitEdge(x);

        public virtual TResult VisitCFGForeachMoveNextEdge(Graph.ForeachMoveNextEdge x) => DefaultVisitEdge(x);

        public virtual TResult VisitCFGSwitchEdge(Graph.SwitchEdge x) => DefaultVisitEdge(x);

        #endregion
    }
}
