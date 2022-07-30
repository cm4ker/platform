using Aquila.CodeAnalysis.FlowAnalysis;
using Aquila.CodeAnalysis.Semantics.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquila.CodeAnalysis.Symbols
{
    partial class SourceMethodSymbol
    {
        /// <summary>
        /// Method flags lazily collected during code analysis.
        /// </summary>
        internal MethodFlags Flags { get; set; }

        /// <summary>
        /// Marks whether the exit block or any block with a return statement was already processed at least once.
        /// </summary>
        internal bool IsReturnAnalysed
        {
            get => (Flags & MethodFlags.IsReturnAnalysed) != 0;
            set
            {
                if (value)
                    Flags |= MethodFlags.IsReturnAnalysed;
                else
                    Flags &= ~MethodFlags.IsReturnAnalysed;
            }
        }
    }
}
