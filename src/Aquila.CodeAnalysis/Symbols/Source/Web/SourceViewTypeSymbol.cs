using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Aquila.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Emit;
using Aquila.CodeAnalysis.FlowAnalysis;
using Aquila.CodeAnalysis.Semantics.Graph;
using Aquila.CodeAnalysis.Syntax;
using Aquila.CodeAnalysis.Web;
using Microsoft.CodeAnalysis;
using Roslyn.Utilities;


namespace Aquila.CodeAnalysis.Symbols.Source;

internal class SourceViewTypeSymbol : NamedTypeSymbol
{
    private readonly NamespaceOrTypeSymbol _container;
    private readonly AquilaSyntaxTree _tree;
    private readonly PEModuleBuilder _builder;
    private readonly CompilationUnitSyntax _cu;
    private readonly SynthesizedManager _smrg;
    private readonly CoreTypes _ct;

    private ImmutableArray<Symbol> _members;


    public SourceViewTypeSymbol(NamespaceOrTypeSymbol container, AquilaSyntaxTree tree, PEModuleBuilder builder)
    {
        _container = container;
        _tree = tree;
        _builder = builder;
        _cu = tree.GetCompilationUnitRoot();
        _ct = container.DeclaringCompilation.CoreTypes;
    }

    internal override ObsoleteAttributeData ObsoleteAttributeData { get; }
    public override Symbol ContainingSymbol => _container;
    public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences { get; }
    public override Accessibility DeclaredAccessibility => Accessibility.Public;
    public override bool IsStatic => false;

    public override ImmutableArray<CustomModifier> GetTypeArgumentCustomModifiers(int ordinal)
    {
        return ImmutableArray<CustomModifier>.Empty;
    }

    public override int Arity => 0;
    public override bool IsSerializable => false;
    internal override bool MangleName => false;

    internal override ImmutableArray<NamedTypeSymbol> GetDeclaredInterfaces(ConsList<Symbol> basesBeingResolved)
    {
        return ImmutableArray<NamedTypeSymbol>.Empty;
    }

    internal override bool IsInterface => false;
    internal override bool HasTypeArgumentsCustomModifiers => false;

    internal override IEnumerable<IFieldSymbol> GetFieldsToEmit()
    {
        return ImmutableArray<IFieldSymbol>.Empty;
    }

    internal override ImmutableArray<NamedTypeSymbol> GetInterfacesToEmit()
    {
        return ImmutableArray<NamedTypeSymbol>.Empty;
    }

    public override bool IsAbstract => false;
    internal override bool IsWindowsRuntimeImport => false;
    internal override bool ShouldAddWinRTMembers => false;
    public override bool IsSealed => true;

    internal override TypeLayout Layout { get; }

    public override ImmutableArray<Symbol> GetMembers()
    {
        if (_members == null)
        {
            var builder = ImmutableArray.CreateBuilder<Symbol>();
            builder.Add(new MethodTreeBuilderSymbol(this, _cu.Html));

            _members = builder.ToImmutable();
        }

        return _members;
    }

    public override ImmutableArray<Symbol> GetMembers(string name)
    {
        return GetMembers().Where(x => x.Name == name).ToImmutableArray();
    }

    public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
    {
        return ImmutableArray<NamedTypeSymbol>.Empty;
    }

    public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
    {
        return ImmutableArray<NamedTypeSymbol>.Empty;
    }

    public override TypeKind TypeKind => TypeKind.Class;
}

internal class MethodTreeBuilderSymbol : MethodSymbol
{
    private readonly NamedTypeSymbol _type;
    private readonly HtmlDecl _htmlDecl;

    ControlFlowGraph _cfg;
    private readonly CoreTypes _ct;
    private readonly CoreType _componentBaseType;
    private readonly MethodSymbol _overridenMethod;

    public MethodTreeBuilderSymbol(NamedTypeSymbol type, HtmlDecl htmlDecl)
    {
        Contract.ThrowIfNull(type);

        _type = type;
        _htmlDecl = htmlDecl;

        _ct = _type.DeclaringCompilation.CoreTypes;

        _componentBaseType = _ct.Web_ComponentBase;
        _overridenMethod = _componentBaseType.Method("BuildRenderTree", _ct.Web_RenderTreeBuilder).Symbol;
    }

    public override IMethodSymbol OverriddenMethod => _overridenMethod;
    public override string Name => _overridenMethod.Name;
    internal override ObsoleteAttributeData ObsoleteAttributeData => _overridenMethod.ObsoleteAttributeData;
    public override Symbol ContainingSymbol => this._type;
    public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;
    public override Accessibility DeclaredAccessibility => Accessibility.Public;
    public override bool IsStatic => _overridenMethod.IsStatic;
    public override bool IsVirtual => _overridenMethod.IsVirtual;

    internal override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false)
    {
        return false;
    }

    public override ImmutableArray<ParameterSymbol> Parameters { get; }

    private void GetParametersCore()
    {
    }

    public override bool ReturnsVoid => _overridenMethod.ReturnsVoid;
    public override RefKind RefKind => _overridenMethod.RefKind;
    public override TypeSymbol ReturnType => _overridenMethod.ReturnType;
    public override bool IsOverride => true;
    public override MethodKind MethodKind => MethodKind.Ordinary;
    public override bool IsAbstract => false;

    internal override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
    {
        return false;
    }

    public override bool IsSealed => true;
    public override bool IsExtern => false;

    /// <summary>
    /// Lazily bound semantic block.
    /// Entry point of analysis and emitting.
    /// </summary>
    public override ControlFlowGraph ControlFlowGraph
    {
        get
        {
            if (_cfg == null)
            {
                var binder = DeclaringCompilation.GetBinder(_htmlDecl);
                var cfg = new ControlFlowGraph(this._htmlDecl.HtmlNodes, binder);
                Interlocked.CompareExchange(ref _cfg, cfg, null);
            }

            //
            return _cfg;
        }
        internal set { _cfg = value; }
    }
}