using System.Collections.Immutable;
using System.Linq;
using Aquila.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Aquila.CodeAnalysis.Semantics;

internal class GlobalBinder : InContainerBinder
{
    private readonly NamespaceOrTypeSymbol _container;

    public GlobalBinder(INamespaceOrTypeSymbol ns, Binder next) : base(ns, next)
    {
        _container = (NamespaceOrTypeSymbol)ns;
    }

    public override NamespaceOrTypeSymbol Container => _container;

    protected override void FindMethodsByName(string name, ArrayBuilder<Symbol> result)
    {
        result.AddRange(_container.GetMembers(name).OfType<MethodSymbol>());
    }

    protected override void FindSymbolByName(string name, ArrayBuilder<ImmutableArray<Symbol>> result,
        FilterCriteria filterCriteria)
    {
        FindSymbolByNameHandler(
            _container.GetMembers(name)
                .Union(_container.GetTypeMembers(name, -1), SymbolEqualityComparer.Default).Cast<Symbol>(),
            result, filterCriteria, () => { });
    }
}