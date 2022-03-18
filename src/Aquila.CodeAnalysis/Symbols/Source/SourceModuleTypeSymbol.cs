using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Aquila.CodeAnalysis.Syntax;
using Aquila.Syntax.Declarations;
using Microsoft.CodeAnalysis;
using MoreLinq;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Symbols.Source;

internal class SourceModuleTypeSymbol : NamedTypeSymbol
{
    private readonly NamespaceOrTypeSymbol _container;
    private readonly MergeModuleDecl _module;
    private ImmutableArray<Symbol> _members;
    private ImmutableArray<SourceTypeSymbol> _types;

    public SourceModuleTypeSymbol(NamespaceOrTypeSymbol container, MergeModuleDecl module)
    {
        _container = container;
        _module = module;
    }

    internal override ObsoleteAttributeData ObsoleteAttributeData { get; }

    public override Symbol ContainingSymbol => _container;

    public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences { get; }

    public override Accessibility DeclaredAccessibility => Accessibility.Internal;

    public override bool IsStatic => true;

    public override ImmutableArray<CustomModifier> GetTypeArgumentCustomModifiers(int ordinal)
    {
        return ImmutableArray<CustomModifier>.Empty;
    }

    public override string Name => _module.Name;

    public override int Arity { get; }
    public override bool IsSerializable { get; }
    internal override bool MangleName { get; }
    
    public override NamedTypeSymbol BaseType => DeclaringCompilation.GetSpecialType(SpecialType.System_Object);

    internal override ImmutableArray<NamedTypeSymbol> GetDeclaredInterfaces(ConsList<Symbol> basesBeingResolved)
    {
        return ImmutableArray<NamedTypeSymbol>.Empty;
    }

    internal override bool IsInterface { get; }
    internal override bool HasTypeArgumentsCustomModifiers { get; }

    internal override IEnumerable<IFieldSymbol> GetFieldsToEmit()
    {
        return GetMembers().OfType<IFieldSymbol>();
    }

    internal override ImmutableArray<NamedTypeSymbol> GetInterfacesToEmit()
    {
        return ImmutableArray<NamedTypeSymbol>.Empty;
    }

    public override bool IsAbstract { get; }
    internal override bool IsWindowsRuntimeImport { get; }
    internal override bool ShouldAddWinRTMembers { get; }
    public override bool IsSealed { get; }
    internal override TypeLayout Layout { get; }


    private ImmutableArray<Symbol> EnsureMembersCore()
    {
        if (_members == null || _members.IsDefault)
            _members = _module.ModuleFunctions.Select(x => (Symbol)new SourceMethodSymbol(this, x)).ToImmutableArray();

        return _members;
    }

    private ImmutableArray<NamedTypeSymbol> EnsureInternalTypes()
    {
        if (_types == null || _types.IsDefault)
        {
            _types = _module.Types.Select(x => new SourceTypeSymbol(this, x)).ToImmutableArray();
        }

        return _types.CastArray<NamedTypeSymbol>();
    }


    public override ImmutableArray<Symbol> GetMembers()
    {
        return EnsureMembersCore();
    }

    public override ImmutableArray<Symbol> GetMembers(string name)
    {
        return GetMembers().Where(x => x.Name == name).ToImmutableArray();
    }

    public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
    {
        return EnsureInternalTypes();
    }

    public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
    {
        return GetTypeMembers().Where(x => x.Name == name).ToImmutableArray();
    }

    public override TypeKind TypeKind => TypeKind.Class;


    public void GetDiagnostics(DiagnosticBag diagnostic)
    {
        foreach (var namedTypeSymbol in EnsureInternalTypes())
        {
            var type = (SourceTypeSymbol)namedTypeSymbol;
            type.GetDiagnostics(diagnostic);
        }
    }
}