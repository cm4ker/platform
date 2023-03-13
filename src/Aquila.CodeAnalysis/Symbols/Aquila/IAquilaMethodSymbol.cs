using Aquila.CodeAnalysis.Semantics.Graph;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Symbols.Aquila
{
    /// <summary>
    /// A symbol representing method in CLR.
    /// </summary>
    public interface IAquilaMethodSymbol : IMethodSymbol
    {
        /// <summary>
        /// For source routines, gets their control flow graph.
        /// Can be <c>null</c> for routines from PE.
        /// </summary>
        ControlFlowGraph ControlFlowGraph { get; }
    }
}