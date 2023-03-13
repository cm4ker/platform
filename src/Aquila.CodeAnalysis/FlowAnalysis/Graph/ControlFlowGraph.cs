using Aquila.CodeAnalysis.FlowAnalysis;

namespace Aquila.CodeAnalysis.Semantics.Graph
{
    partial class ControlFlowGraph
    {
        public bool HasFlowState => this.Start.FlowState != null;

        /// <summary>
        /// Gets flow analysis context for this CFG.
        /// </summary>
        /// <remarks>CFG has to be analysed prior to getting this property.</remarks>
        public FlowContext FlowContext => this.Start.FlowState?.FlowContext;
    }
}