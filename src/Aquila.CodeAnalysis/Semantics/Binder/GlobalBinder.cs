using System.Collections.Immutable;
using Aquila.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Aquila.CodeAnalysis.Semantics;

internal class GlobalBinder : InContainerBinder
{
    private readonly NamespaceOrTypeSymbol _ns;

    public GlobalBinder(INamespaceOrTypeSymbol ns, Binder next) : base(ns, next)
    {
        _ns = (NamespaceOrTypeSymbol)ns;
    }

    public override NamespaceOrTypeSymbol ContainingType => _ns;

    protected override void FindMethodsByName(string name, ArrayBuilder<Symbol> result)
    {
        result.AddRange(_ns.GetMembers(name).OfType<MethodSymbol>());
    }

    protected override void FindSymbolByName(string name, ArrayBuilder<ImmutableArray<Symbol>> result,
        FilterCriteria filterCriteria)
    {   
        FindSymbolByNameHandler(_ns.GetMembers(name), result, filterCriteria);
    }
}