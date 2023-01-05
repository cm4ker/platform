using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Aquila.CodeAnalysis.Semantics.Graph;
using Aquila.CodeAnalysis.Syntax;
using Microsoft.CodeAnalysis;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Symbols.Source;

/// <summary>
/// Razor component symbol type from the view file. Base from <see cref="ComponentBase"/>
/// </summary>
internal class SourceViewTypeSymbol : NamedTypeSymbol
{
    private readonly NamespaceOrTypeSymbol _container;
    private readonly CompilationUnitSyntax _cu;
    private readonly HtmlDecl _htmlDecl;
    private readonly CoreTypes _ct;
    private ImmutableArray<Symbol> _members;

    public SourceViewTypeSymbol(NamespaceOrTypeSymbol container, AquilaSyntaxTree tree)
    {
        _container = container;
        _cu = tree.GetCompilationUnitRoot();
        _ct = container.DeclaringCompilation.CoreTypes;
        _htmlDecl = _cu.Html ?? throw new ArgumentException("Syntax tree for view type must contains the html node tree", nameof(tree));
    }

    internal override ObsoleteAttributeData ObsoleteAttributeData { get; }
    public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences { get; }
    public override Symbol ContainingSymbol => _container;
    public override Accessibility DeclaredAccessibility => Accessibility.Public;
    public override bool IsStatic => false;
    public override int Arity => 0;
    public override bool IsSerializable => false;
    internal override bool MangleName => false;
    public override NamedTypeSymbol BaseType => _ct.Web_ComponentBase;
    internal override bool IsInterface => false;
    internal override bool HasTypeArgumentsCustomModifiers => false;
    public override bool IsAbstract => false;
    internal override bool IsWindowsRuntimeImport => false;
    internal override bool ShouldAddWinRTMembers => false;
    public override bool IsSealed => true;
    internal override TypeLayout Layout { get; }
    public override TypeKind TypeKind => TypeKind.Class;


    public override ImmutableArray<CustomModifier> GetTypeArgumentCustomModifiers(int ordinal)
    {
        return ImmutableArray<CustomModifier>.Empty;
    }

    internal override ImmutableArray<NamedTypeSymbol> GetDeclaredInterfaces(ConsList<Symbol> basesBeingResolved)
    {
        return ImmutableArray<NamedTypeSymbol>.Empty;
    }

    internal override IEnumerable<IFieldSymbol> GetFieldsToEmit()
    {
        return ImmutableArray<IFieldSymbol>.Empty;
    }

    internal override ImmutableArray<NamedTypeSymbol> GetInterfacesToEmit()
    {
        return ImmutableArray<NamedTypeSymbol>.Empty;
    }

    public override ImmutableArray<Symbol> GetMembers()
    {
        if (_members == null)
        {
            var builder = ImmutableArray.CreateBuilder<Symbol>();
            builder.Add(new MethodTreeBuilderSymbol(this, _cu.Html));
            
            if(_htmlDecl.HtmlCode != null)
                builder.AddRange(_htmlDecl.HtmlCode.Functions.Select(x => new SourceMethodSymbol(this, x)));

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


    internal class MethodTreeBuilderSymbol : MethodSymbol
    {
        private readonly NamedTypeSymbol _type;
        private readonly HtmlDecl _htmlDecl;

        private ControlFlowGraph _cfg;
        private readonly MethodSymbol _overridenMethod;

        public MethodTreeBuilderSymbol(NamedTypeSymbol type, HtmlDecl htmlDecl)
        {
            Contract.ThrowIfNull(type);

            _type = type;
            _htmlDecl = htmlDecl;

            var ct = _type.DeclaringCompilation.CoreTypes;
            var componentBaseType = ct.Web_ComponentBase;

            _overridenMethod = componentBaseType.Method("BuildRenderTree", ct.Web_RenderTreeBuilder).Symbol;
        }

        public override IMethodSymbol OverriddenMethod => _overridenMethod;
        public override string Name => _overridenMethod.Name;
        internal override ObsoleteAttributeData ObsoleteAttributeData => _overridenMethod.ObsoleteAttributeData;
        public override Symbol ContainingSymbol => this._type;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences =>
            ImmutableArray<SyntaxReference>.Empty;

        public override Accessibility DeclaredAccessibility => Accessibility.Public;
        public override bool IsStatic => _overridenMethod.IsStatic;
        public override bool IsVirtual => _overridenMethod.IsVirtual;

        internal override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false)
        {
            return false;
        }

        public override ImmutableArray<ParameterSymbol> Parameters { get; }

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

        ///<inheritdoc />
        public override ControlFlowGraph ControlFlowGraph
        {
            get
            {
                if (_cfg != null) return _cfg;

                var binder = DeclaringCompilation.GetBinder(_htmlDecl);
                var cfg = new ControlFlowGraph(this._htmlDecl.HtmlNodes, binder);
                Interlocked.CompareExchange(ref _cfg, cfg, null);

                return _cfg;
            }
            internal set => _cfg = value;
        }
    }
}