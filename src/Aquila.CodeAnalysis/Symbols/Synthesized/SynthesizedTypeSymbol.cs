using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Aquila.Syntax.Parser;
using Microsoft.CodeAnalysis;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Symbols.Synthesized
{
    /// <summary>
    /// internal static class &lt;Script&gt; { ... }
    /// </summary>
    partial class SynthesizedTypeSymbol : NamedTypeSymbol
    {
        readonly AquilaCompilation _compilation;

        /// <summary>
        /// Optional. Real assembly entry point method.
        /// </summary>
        internal MethodSymbol EntryPointSymbol { get; set; }

        /// <summary>
        /// Additional type members.
        /// </summary>
        private List<Symbol> _lazyMembers = new List<Symbol>();

        private bool _isAbstract;
        private bool _isStatic;
        private string _name;
        private string _namespace;
        private NamedTypeSymbol _baseType;

        public SynthesizedTypeSymbol(AquilaCompilation compilation)
        {
            _compilation = compilation;
        }

        #region Setters

        public void SetIsAbstract(bool value)
        {
            _isAbstract = value;
        }

        public void SetIsStatic(bool value)
        {
            _isStatic = value;
        }

        public void SetName(string value)
        {
            _name = value;
        }

        public void SetNamespace(string value)
        {
            _namespace = value;
        }

        public void SetNamespace(INamedTypeSymbol value)
        {
            _baseType = (NamedTypeSymbol) value;
        }

        #endregion

        public override int Arity => 0;

        internal override bool HasTypeArgumentsCustomModifiers => false;

        public override ImmutableArray<CustomModifier> GetTypeArgumentCustomModifiers(int ordinal) =>
            GetEmptyTypeArgumentCustomModifiers(ordinal);

        public override Symbol ContainingSymbol => _compilation.SourceModule;

        internal override IModuleSymbol ContainingModule => _compilation.SourceModule;

        public override Accessibility DeclaredAccessibility => Accessibility.Internal;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get { throw new NotImplementedException(); }
        }

        public override bool IsAbstract => _isAbstract;

        public override bool IsSealed => false;

        public override bool IsStatic => _isStatic;

        public override bool IsSerializable => false;

        public override string Name => _name;

        public override string NamespaceName => _namespace;

        public override NamedTypeSymbol BaseType => _baseType;

        public override TypeKind TypeKind => TypeKind.Class;

        internal override bool IsInterface => false;

        internal override bool IsWindowsRuntimeImport => false;

        internal override TypeLayout Layout => default(TypeLayout);

        internal override bool MangleName => false;

        internal override ObsoleteAttributeData ObsoleteAttributeData => null;

        internal override bool ShouldAddWinRTMembers => false;

        public override ImmutableArray<Symbol> GetMembers()
        {
            return _lazyMembers.AsImmutable();
        }

        public override ImmutableArray<Symbol> GetMembers(string name) =>
            GetMembers().Where(m => m.Name == name).AsImmutable();

        public override ImmutableArray<Symbol> GetMembersByPhpName(string name) => ImmutableArray<Symbol>.Empty;

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers() =>
            _lazyMembers.OfType<NamedTypeSymbol>().AsImmutable();

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name) => _lazyMembers
            .OfType<NamedTypeSymbol>().Where(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            .AsImmutable();

        internal override ImmutableArray<NamedTypeSymbol> GetDeclaredInterfaces(ConsList<Symbol> basesBeingResolved) =>
            ImmutableArray<NamedTypeSymbol>.Empty;

        internal override IEnumerable<IFieldSymbol> GetFieldsToEmit() =>
            _lazyMembers.OfType<FieldSymbol>().AsImmutable();

        internal override ImmutableArray<NamedTypeSymbol> GetInterfacesToEmit() =>
            ImmutableArray<NamedTypeSymbol>.Empty;

        public override ImmutableArray<MethodSymbol> StaticConstructors => ImmutableArray<MethodSymbol>.Empty;
    }
}