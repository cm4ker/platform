using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.FlowAnalysis;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Source;

namespace Aquila.CodeAnalysis.FlowAnalysis.Passes
{
    /// <summary>
    /// Stores certain types of transformations in parallel fashion and performs them serially afterwards.
    /// </summary>
    internal class DelayedTransformations
    {
        /// <summary>
        /// Methods with unreachable declarations.
        /// </summary>
        public ConcurrentBag<SourceMethodSymbolBase> UnreachableMethods { get; } = new ConcurrentBag<SourceMethodSymbolBase>();

        public bool Apply()
        {
            bool changed = false;

            foreach (var method in UnreachableMethods)
            {
                if (!method.IsUnreachable)
                {
                    method.Flags |= MethodFlags.IsUnreachable;
                    changed = true;
                }
            }

            return changed;
        }
    }
}
