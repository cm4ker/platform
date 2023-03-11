using System;
using System.Collections.Immutable;
using System.Linq;
using Aquila.CodeAnalysis.Semantics;
using Aquila.Compiler.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Aquila.CodeAnalysis.Symbols.Source;

internal class SourceGlobalNamespaceSymbol : NamespaceSymbol
{
    readonly SourceModuleSymbol _sourceModule;
    private ImmutableArray<MethodSymbol> _exntensionMethods;
    private ImmutableArray<NamedTypeSymbol> _userVisibleTypes;


    public SourceGlobalNamespaceSymbol(SourceModuleSymbol module)
    {
        _sourceModule = module;
    }

    internal override NamespaceExtent Extent
    {
        get { return new NamespaceExtent(_sourceModule); }
    }

    internal override AquilaCompilation DeclaringCompilation => _sourceModule.DeclaringCompilation;

    public override AquilaCompilation ContainingCompilation => _sourceModule.DeclaringCompilation;

    public override void Accept(SymbolVisitor visitor) => visitor.VisitNamespace(this);

    public override TResult Accept<TResult>(SymbolVisitor<TResult> visitor) => visitor.VisitNamespace(this);

    public override AssemblySymbol ContainingAssembly => _sourceModule.ContainingAssembly;

    internal override ModuleSymbol ContainingModule => _sourceModule;

    public override Symbol ContainingSymbol => _sourceModule;

    public override string Name => string.Empty;

    public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
    {
        get { throw new NotImplementedException(); }
    }

    public override ImmutableArray<Location> Locations
    {
        get { throw new NotImplementedException(); }
    }

    private void EnsureExtensionMethods()
    {
        if (!_exntensionMethods.IsDefault)
            return;

        var builder = new ArrayBuilder<MethodSymbol>();

        var syms = DeclaringCompilation.GetBoundReferenceManager().ExplicitReferencesSymbols;
        foreach (AssemblySymbol sym in syms)
        {
            var gns = sym.Modules[0].GlobalNamespace;

            HandleNs(gns);

            void HandleNs(NamespaceSymbol ns)
            {
                var members = ns.GetMembers();

                foreach (var member in members)
                {
                    if (member.Kind == SymbolKind.Namespace)
                    {
                        var nestedNs = (NamespaceSymbol)member;
                        HandleNs(nestedNs);
                    }

                    if (member.Kind == SymbolKind.NamedType &&
                        member.GetAttribute(CoreTypes.AquilaExtensionAqAttributeFullName) != null)
                    {
                        var ext = (NamedTypeSymbol)member;
                        var methods = ext.GetMembers().OfType<MethodSymbol>()
                            .Where(x => x.IsStatic && x.DeclaredAccessibility == Accessibility.Public);

                        builder.AddRange(methods);
                    }
                }
            }
        }

        _exntensionMethods = builder.ToImmutableAndFree();
    }

    private void EnsureUserVisibleTypes()
    {
        if (!_userVisibleTypes.IsDefault)
            return;

        var builder = new ArrayBuilder<NamedTypeSymbol>();

        var syms = DeclaringCompilation.GetBoundReferenceManager().ExplicitReferencesSymbols;
        foreach (AssemblySymbol sym in syms)
        {
            var gns = sym.Modules[0].GlobalNamespace;

            HandleNs(gns);

            void HandleNs(NamespaceSymbol ns)
            {
                var members = ns.GetMembers();

                foreach (var member in members)
                {
                    if (member.Kind == SymbolKind.Namespace)
                    {
                        var nestedNs = (NamespaceSymbol)member;
                        HandleNs(nestedNs);
                    }

                    if (member.Kind == SymbolKind.NamedType &&
                        member.GetAttribute(CoreTypes.AquilaUserVisibleAttributeFullName) != null)
                    {
                        builder.AddRange((NamedTypeSymbol)member);
                    }
                }
            }
        }

        _userVisibleTypes = builder.ToImmutableAndFree();
    }

    private ImmutableArray<Symbol> _referencedMembers;

    private void EnsureReferencedMembers()
    {
        if (!_referencedMembers.IsDefault)
            return;

        var builder = new ArrayBuilder<Symbol>();

        var syms = DeclaringCompilation.GetBoundReferenceManager().ExplicitReferencesSymbols;
        foreach (AssemblySymbol sym in syms)
        {
            var gns = sym.Modules[0].GlobalNamespace;
            builder.AddRange(gns.GetMembers());
        }

        _referencedMembers = builder.ToImmutableAndFree();
    }

    private void EnsureModulesTypes()
    {
    }

    public override ImmutableArray<Symbol> GetMembers()
    {
        EnsureExtensionMethods();
        EnsureUserVisibleTypes();
        
        var arr = new ArrayBuilder<Symbol>();

        var platformSymbols = DeclaringCompilation.PlatformSymbolCollection.GetNamespaces().OfType<Symbol>();
        
        arr.AddRange(platformSymbols);

        var modules = DeclaringCompilation.SourceSymbolCollection.GetModuleTypes();
        arr.AddRange(modules);

        var result = _exntensionMethods;
        arr.AddRange(result);

        return arr.ToImmutableAndFree();
    }


    public override ImmutableArray<Symbol> GetMembers(string name)
    {
        return GetMembers().Where(x => x.Name == name).ToImmutableArray();
    }

    public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
    {
        EnsureUserVisibleTypes();
        return _sourceModule.DeclaringCompilation.PlatformSymbolCollection.GetAllCreatedTypes().AsImmutable();
    }

    public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
    {
        EnsureUserVisibleTypes();

        var x = _sourceModule.SymbolCollection.TryGetType(NameUtils.MakeQualifiedName(name, true));
        if (x != null)
        {
            if (x.IsErrorType())
            {
                var candidates = ((ErrorTypeSymbol)x).CandidateSymbols;
                if (candidates.Length != 0)
                {
                    return candidates.OfType<NamedTypeSymbol>().AsImmutable();
                }
            }
            else
            {
                return ImmutableArray.Create(x);
            }
        }

        var builder = new ArrayBuilder<NamedTypeSymbol>();
        var types = _userVisibleTypes.Where(t => t.Name == name).ToList();

        //force return result
        if (types.Any())
        {
            builder.AddRange(types);
            return builder.ToImmutableArray();
        }

        var type = _sourceModule.DeclaringCompilation.PlatformSymbolCollection
            .TryGetType(QualifiedName.Parse(name, false));

        if (type != null)
            builder.Add(type);


        return builder.ToImmutableAndFree();
    }
}