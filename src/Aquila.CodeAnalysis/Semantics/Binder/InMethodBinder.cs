using System;
using System.Collections.Immutable;
using System.Linq;
using Aquila.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Semantics;

internal sealed class InLambdaBinder : InMethodBinder
{
    public InLambdaBinder(SourceLambdaSymbol lambda, Binder next) : base(lambda, next)
    {
    }

    public override bool IsInLambda => true;
}

internal class InMethodBinder : Binder
{
    private readonly SourceMethodSymbolBase _method;

    public InMethodBinder(MethodSymbol method, Binder next) : base(next)
    {
        if (method is null)
            throw new ArgumentNullException(nameof(method));

        if (method is not SourceMethodSymbolBase m)
            throw new ArgumentException($@"The method must be {nameof(SourceMethodSymbolBase)} type", nameof(method));

        _method = m;
    }

    public override SourceMethodSymbolBase Method => _method;


    public override NamespaceOrTypeSymbol ContainingType => _method.ContainingType;

    public override Symbol ContainingSymbol => _method.ContainingSymbol;

    protected override void FindSymbolByName(string name, ArrayBuilder<ImmutableArray<Symbol>> result,
        FilterCriteria filterCriteria)
    {
        FindSymbolByNameHandler(Locals
            .Variables
            .Where(x => x.Name == name)
            .Select(x => x.Symbol)
            .WhereNotNull(), result, filterCriteria);
        base.FindSymbolByName(name, result, filterCriteria);
    }
}