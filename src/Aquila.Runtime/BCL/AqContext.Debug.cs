using System.Diagnostics;

namespace Aquila.Core
{
    [DebuggerDisplay("{DebugDisplay,nq}")]
    [DebuggerTypeProxy(typeof(ContextDebugView))]
    [DebuggerNonUserCode]
    partial class AqContext
    {
        /// <summary>The debug view text.</summary>
        protected virtual string DebugDisplay => "Context";

        /// <summary>Content in debug view.</summary>
        sealed class ContextDebugView
        {
            readonly AqContext _ctx;

            public ContextDebugView(AqContext ctx)
            {
                _ctx = ctx;
            }
        }
    }
}