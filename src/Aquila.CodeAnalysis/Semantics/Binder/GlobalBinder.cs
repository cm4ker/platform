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

    public override NamespaceOrTypeSymbol Container => _ns;

    protected override void FindMethodsByName(string name, ArrayBuilder<Symbol> result)
    {
        result.AddRange(_ns.GetMembers(name).OfType<MethodSymbol>());
    }
}