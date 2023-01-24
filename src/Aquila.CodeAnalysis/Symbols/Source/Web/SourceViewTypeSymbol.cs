using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using Aquila.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.FlowAnalysis;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Semantics.Graph;
using Aquila.CodeAnalysis.Symbols.Attributes;
using Aquila.CodeAnalysis.Syntax;
using Microsoft.CodeAnalysis;
using Roslyn.Utilities;
using TypeLayout = Microsoft.CodeAnalysis.TypeLayout;


namespace Aquila.CodeAnalysis.Symbols.Source;

/// <summary>
/// Razor component symbol type from the view file. Base from <see cref="ComponentBase"/>
/// </summary>
internal class SourceViewTypeSymbol : NamedTypeSymbol
{
    private readonly NamespaceOrTypeSymbol _container;
    private readonly HtmlDecl _htmlDecl;
    private readonly CoreTypes _ct;
    private ImmutableArray<Symbol> _members;
    private readonly string _name;
    private ImmutableArray<AttributeData> _attributeData;

    public SourceViewTypeSymbol(NamespaceOrTypeSymbol container, HtmlDecl htmlDecl)
    {
        
        _container = container;
        _ct = container.DeclaringCompilation.CoreTypes;
        _htmlDecl = htmlDecl;
        if (!string.IsNullOrEmpty(_htmlDecl.SyntaxTree.FilePath))
            _name = TranslateViewNameFromFileName(Path.GetFileNameWithoutExtension(_htmlDecl.SyntaxTree.FilePath));
        else
            _name = $"View_{Guid.NewGuid()}";
    }

    private static string TranslateViewNameFromFileName(string fileName)
        => fileName.Split(new[] { "_", "-" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => char.ToUpperInvariant(s[0]) + s.Substring(1, s.Length - 1))
            .Aggregate(string.Empty, (s1, s2) => s1 + s2);

    internal HtmlDecl AquilaSyntax => _htmlDecl;
    
    public override string Name => _name;
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
        return GetMembers().OfType<IFieldSymbol>();
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
            builder.Add(new MethodTreeBuilderSymbol(this, _htmlDecl));
            
            SourceTypeSymbolHelper.AddDefaultInstanceTypeSymbolMembers(this, builder);

            if (_htmlDecl.HtmlCode != null)
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

    private void EnsureAttributes()
    {
        if (!_attributeData.IsDefault)
            return;

        _attributeData = new AttributeData[]
        {
            new SynthesizedAttributeData(_ct.Web_Route.Ctor(_ct.String),
                ImmutableArray.Create(new TypedConstant(_ct.String.Symbol,
                    TypedConstantKind.Primitive, $"/view/{{instanceName}}/{_name.ToCamelCase()}")),
                ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty)
        }.ToImmutableArray();
    }

    public override ImmutableArray<AttributeData> GetAttributes()
    {
        EnsureAttributes();
        return _attributeData;
    }


    internal class MethodTreeBuilderSymbol : SourceMethodSymbolBase
    {
        private readonly NamedTypeSymbol _type;
        private readonly HtmlDecl _htmlDecl;
        private IPlace _builderPlace;

        private readonly MethodSymbol _overridenMethod;

        public MethodTreeBuilderSymbol(NamedTypeSymbol type, HtmlDecl htmlDecl) : base(type)
        {
            Contract.ThrowIfNull(type);

            _type = type;
            _htmlDecl = htmlDecl;

            var ct = _type.DeclaringCompilation.CoreTypes;
            var componentBaseType = ct.Web_ComponentBase;

            _overridenMethod = componentBaseType.Method("BuildRenderTree", ct.Web_RenderTreeBuilder).Symbol;
        }

        public override IMethodSymbol OverriddenMethod => _overridenMethod;
        internal override ParameterListSyntax SyntaxSignature => SyntaxFactory.ParameterList();

        internal override TypeEx SyntaxReturnType =>
            SyntaxFactory.PredefinedTypeEx(SyntaxFactory.Token(SyntaxKind.VoidKeyword));

        internal override AquilaSyntaxNode Syntax => _htmlDecl;
        internal override IEnumerable<StmtSyntax> Statements => new List<StmtSyntax>();
        public override string Name => _overridenMethod.Name;
        internal override ObsoleteAttributeData ObsoleteAttributeData => _overridenMethod.ObsoleteAttributeData;

        public override void GetDiagnostics(DiagnosticBag diagnostic)
        {
        }

        public override Symbol ContainingSymbol => this._type;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences =>
            ImmutableArray<SyntaxReference>.Empty;

        public override Accessibility DeclaredAccessibility => _overridenMethod.DeclaredAccessibility;
        public override bool IsStatic => _overridenMethod.IsStatic;
        public override bool IsVirtual => _overridenMethod.IsVirtual;

        public override bool HidesBaseMethodsByName => true;

        public override bool ReturnsVoid => _overridenMethod.ReturnsVoid;

        public override RefKind RefKind => _overridenMethod.RefKind;

        public override TypeSymbol ReturnType => _overridenMethod.ReturnType;

        public override bool IsOverride => true;

        public override MethodKind MethodKind => MethodKind.Ordinary;

        public override bool IsAbstract => false;

        public override bool IsSealed => true;

        public override bool IsExtern => false;

        protected override Binder GetMethodBinder()
        {
            return DeclaringCompilation.GetBinder(_htmlDecl.HtmlMarkup);
        }

        protected override ControlFlowGraph CreateControlFlowGraph()
        {
            var markup = _htmlDecl.HtmlMarkup;
            if (markup == null) return null;
            
            var cfg = new ControlFlowGraph(markup.HtmlNodes, GetMethodBinder())
            {
                Start =
                {
                    FlowState = StateBinder.CreateInitialState(this)
                }
            };

            return cfg;

        }

        ///<inheritdoc />
        public override ControlFlowGraph ControlFlowGraph
        {
            get
            {
                if (_cfg != null)
                {
                    return _cfg;
                }

                if (_htmlDecl.HtmlMarkup == null)
                {
                    return null;
                }

                
                var binder = 
                var cfg = 
                Interlocked.CompareExchange(ref _cfg, cfg, null);
                
                return _cfg;
            }
            internal set => _cfg = value;
        }

        internal override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false)
        {
            return true;
        }

        internal override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
        {
            return false;
        }

        protected override ImplicitParametersBuilder PrepareImplicitParams()
        {
            var builder = base.PrepareImplicitParams();
            builder.Add(index => new SpecialParameterSymbol(this, DeclaringCompilation.CoreTypes.Web_RenderTreeBuilder,
                SpecialParameterSymbol.BuilderName, index));
            return builder;
        }


        internal IPlace GetBuilderPlace()
        {
            if (_builderPlace != null)
                return _builderPlace;

            var builderParam = this.GetParameters().Single(x => x.Name == SpecialParameterSymbol.BuilderName);
            _builderPlace = new ParamPlace(builderParam);
            return _builderPlace;
        }
    }
}