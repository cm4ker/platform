using Aquila.CodeAnalysis.Errors;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Syntax;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Semantics;

internal class InContainerBinder : Binder
{
    private readonly INamespaceOrTypeSymbol _container;

    public InContainerBinder(INamespaceOrTypeSymbol container, Binder next) : base(next)
    {
        _container = container;
    }

    public override NamespaceOrTypeSymbol Container => (NamespaceOrTypeSymbol)_container;


    protected override ITypeSymbol FindTypeByName(NameEx tref)
    {
        TypeSymbol result = null;

        var qName = tref.GetUnqualifiedName().Identifier.Text;

        var typeMembers = Container.GetTypeMembers(qName, -1);

        if (typeMembers.Length == 1)
            result = typeMembers[0];

        if (typeMembers.Length == 0)
        {
            result = Next?.BindType(tref);
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
}