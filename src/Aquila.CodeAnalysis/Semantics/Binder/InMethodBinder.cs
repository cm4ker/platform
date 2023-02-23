using System;
using System.Collections.Immutable;
using System.Linq;
using Aquila.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Semantics;

internal class InMethodBinder : Binder
{
    private readonly SourceMethodSymbolBase _method;

    public InMethodBinder(MethodSymbol method, Binder next) : base(next)
    {
        if(method is null)
            throw new ArgumentNullException(nameof(method));
            
        if (method is not SourceMethodSymbolBase m)
            throw new ArgumentException($@"The method must be {nameof(SourceMethodSymbolBase)} type", nameof(method));
            
        _method = m;
    }

    public override SourceMethodSymbolBase Method => _method;


    public override NamespaceOrTypeSymbol Container => _method.ContainingType;


    protected override void FindSymbolByName(string name, ArrayBuilder<ImmutableArray<Symbol>> result)
    {
        result.Add(Locals
            .Variables
            .Where(x => x.Name == name)
            .Select(x => x.Symbol)
            .WhereNotNull().ToImmutableArray());
        
        base.FindSymbolByName(name, result);
    }
}