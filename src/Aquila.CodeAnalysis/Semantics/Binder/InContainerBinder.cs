using System.Collections.Immutable;
using System.Linq;
using Aquila.CodeAnalysis.Errors;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Aquila.CodeAnalysis.Semantics;

internal class InContainerBinder : Binder
{
    private readonly NamespaceOrTypeSymbol _container;

    public InContainerBinder(INamespaceOrTypeSymbol container, Binder next) : base(next)
    {
        _container = (NamespaceOrTypeSymbol)container;
    }

    public override NamespaceOrTypeSymbol Container => _container;
    public override Symbol ContainingSymbol => _container.ContainingSymbol;


    protected override ITypeSymbol FindTypeByName(NameEx tref)
    {
        TypeSymbol result = null;

        var qName = tref.GetUnqualifiedName().Identifier.Text;

        var typeMembers = Container.GetTypeMembers(qName, -1);

        if (typeMembers.Length == 1)
            result = typeMembers[0];

        if (typeMembers.Length == 0)
        {
            result = Next?.TryResolveTypeSymbol(tref);
        }

        if (typeMembers.Length > 1)
        {
            Diagnostics.Add(GetLocation(tref), ErrorCode.WRN_UndefinedType,
                $"Expression of type '{tref.GetType().Name}'");
        }

        if (result == null)
        {
            result = new MissingMetadataTypeSymbol(qName, 0, true);
        }

        if (result.IsErrorType())
            Diagnostics.Add(GetLocation(tref), ErrorCode.ERR_TypeNameCannotBeResolved, qName);

        return result;
    }
    
    protected override void FindSymbolByName(string name, ArrayBuilder<ImmutableArray<Symbol>> result,
        FilterCriteria filterCriteria)
    {
        FindSymbolByNameHandler(
            _container.GetMembers(name)
                .Union(_container.GetTypeMembers(name, -1), SymbolEqualityComparer.Default).Cast<Symbol>(),
            result, filterCriteria);
        base.FindSymbolByName(name, result, filterCriteria);
    }
}