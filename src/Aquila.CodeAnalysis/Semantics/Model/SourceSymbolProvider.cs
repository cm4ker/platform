using System.Collections.Generic;
using Aquila.CodeAnalysis.Symbols.Source;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Semantics.Model
{
    internal sealed class SourceSymbolProvider : ISymbolProvider
    {
        readonly SourceSymbolCollection _table;

        public AquilaCompilation Compilation => _table.Compilation;

        public SourceSymbolProvider(SourceSymbolCollection table)
        {
            Contract.ThrowIfNull(table);
            _table = table;
        }


        public INamedTypeSymbol ResolveType(QualifiedName name, Dictionary<QualifiedName, INamedTypeSymbol> resolved) =>
            _table.GetType(name, resolved);
    }
}