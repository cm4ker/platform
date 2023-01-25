using System;
using Aquila.CodeAnalysis.Symbols;

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
}