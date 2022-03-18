using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Symbols;

namespace Aquila.CodeAnalysis.FlowAnalysis
{
    /// <summary>
    /// Binds flow state to a method.
    /// </summary>
    internal static class StateBinder
    {
        /// <summary>
        /// Creates new type context, flow context and flow state for the method.
        /// </summary>
        public static FlowState CreateInitialState(SourceMethodSymbol method, FlowContext flowCtx = null)
        {
            Contract.ThrowIfNull(method);

            // get or create typeCtx
            var typeCtx = method.TypeRefContext;

            if (flowCtx == null)
            {
                // create FlowContext 
                flowCtx = new FlowContext(typeCtx, method);
            }

            // create FlowState
            var state = new FlowState(flowCtx);

            // handle input parameters type
            foreach (var p in method.SourceParameters)
            {
                var local = state.GetLocalHandle(new VariableName(p.Name));
                state.SetLocalType(local, null);
            }

            //
            return state;
        }

        /// <summary>
        /// Initializes <c>$this</c> variable, its type and initialized state.
        /// </summary>
        private static void InitThisVar(FlowContext ctx, FlowState initialState)
        {
            var thisHandle = ctx.GetVarIndex(VariableName.ThisVariableName);
            initialState.SetLocalType(thisHandle, null); // set $this type
            initialState.VisitLocal(thisHandle); // mark as visited (used) to not report as unused
        }
    }
}