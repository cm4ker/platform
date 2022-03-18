using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.Symbols;
using System.Collections.Generic;

namespace Aquila.CodeAnalysis.Semantics
{
    /// <summary>
    /// Used to query semantic questions about the compilation in specific context.
    /// </summary>
    /// <remarks>Use <see cref="SemanticModel"/> once we implement <see cref="SyntaxTree"/>.</remarks>
    internal interface ISymbolProvider
    {
        /// <summary>
        /// Gets declaring compilation.
        /// </summary>
        AquilaCompilation Compilation { get; }

        /// <summary>
        /// Gets type symbol by its name in current context.
        /// Can be <c>null</c> if type cannot be found.
        /// Gets <see cref="AmbiguousErrorTypeSymbol"/> in case of an ambiguity.
        /// </summary>
        INamedTypeSymbol ResolveType(QualifiedName name, Dictionary<QualifiedName, INamedTypeSymbol> resolved = null);
    }
}