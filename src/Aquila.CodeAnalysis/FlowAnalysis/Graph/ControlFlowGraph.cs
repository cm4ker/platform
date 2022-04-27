using Aquila.CodeAnalysis.FlowAnalysis;

namespace Aquila.CodeAnalysis.Semantics.Graph
{
    partial class ControlFlowGraph
    {
        //public bool HasFlowState => this.Start.FlowState != null;

        /// <summary>
        /// Gets flow analysis context for this CFG.
        /// </summary>
        /// <remarks>CFG has to be analysed prior to getting this property.</remarks>
        public FlowContext FlowContext => null; //this.Start.FlowState?.FlowContext;

        /// <summary>
        /// Gets possible types of a local variable.
        /// </summary>
        /// <remarks>CFG has to be analysed prior to getting this property.</remarks>
        internal FlowContext.TypeRefInfo GetLocalType(string varname) =>
            this.FlowContext?.GetVarType(new VariableName(varname));
    }
}