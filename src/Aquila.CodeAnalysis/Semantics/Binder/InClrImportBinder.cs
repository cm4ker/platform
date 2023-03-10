using System.Collections.Immutable;
using System.Linq;
using Aquila.CodeAnalysis.Errors;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Syntax;
using Aquila.CodeAnalysis.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Aquila.CodeAnalysis.Semantics;

internal class InClrImportBinder : Binder
{
    private readonly NamespaceOrTypeSymbol _container;

    public override Symbol ContainingSymbol => _container.ContainingSymbol;

    public InClrImportBinder(INamespaceOrTypeSymbol container, Binder next) : base(next)
    {
        Contract.ThrowIfNull(container);
        _container = (NamespaceOrTypeSymbol)container;
    }

    protected override ITypeSymbol FindTypeByName(NameEx tref)
    {
        var qName = tref.GetUnqualifiedName().Identifier.Text;

        var typeMembers = Container.GetTypeMembers(qName, -1);

        if (typeMembers.Length == 1)
            return typeMembers[0];

        if (typeMembers.Length == 0)
        {
            return Next.TryResolveTypeSymbol(tref);
        }

        if (typeMembers.Length > 1)
        {
            Diagnostics.Add(GetLocation(tref), ErrorCode.WRN_UndefinedType,
                $"Expression of type '{tref.GetType().Name}'");
        }

        return null;
    }

    protected override void FindMethodsByName(string name, ArrayBuilder<Symbol> result)
    {
        var typesCandidate = _container.GetTypeMembers().Where(x => name.StartsWith(x.Name.ToSnakeCase()));

        foreach (var type in typesCandidate)
        {
            var typeSnake = type.Name.ToSnakeCase();

            var resolvedMethods = type.GetMembers().Where(x =>
                    x.DeclaredAccessibility == Accessibility.Public && x.IsStatic &&
                    typeSnake + "_" + x.Name.ToSnakeCase() == name)
                .OfType<MethodSymbol>();

            result.AddRange(resolvedMethods);
        }

        base.FindMethodsByName(name, result);
    }

    protected override void FindSymbolByName(string name, ArrayBuilder<ImmutableArray<Symbol>> result,
        FilterCriteria filterCriteria)
    {
        ArrayBuilder<Symbol> methods = new ArrayBuilder<Symbol>();
        FindMethodsByName(name, methods);
        FindSymbolByNameHandler(
            _container.GetMembers(name)
                .Union(_container.GetTypeMembers(name, -1), SymbolEqualityComparer.Default)
                .Union(methods, SymbolEqualityComparer.Default).Cast<Symbol>(), result,
            filterCriteria);
        base.FindSymbolByName(name, result, filterCriteria);
    }
}