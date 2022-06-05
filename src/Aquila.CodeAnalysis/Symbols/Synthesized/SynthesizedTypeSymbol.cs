using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Symbols.Synthesized
{
    /// <summary>
    /// internal static class &lt;Script&gt; { ... }
    /// </summary>
    partial class SynthesizedTypeSymbol : NamedTypeSymbol
    {
        private readonly NamespaceOrTypeSymbol _container;
        readonly AquilaCompilation _compilation;

        /// <summary>
        /// Optional. Real assembly entry point method.
        /// </summary>
        internal MethodSymbol EntryPointSymbol { get; set; }

        /// <summary>
        /// Additional type members.
        /// </summary>
        private ConcurrentBag<Symbol> _lazyMembers = new ConcurrentBag<Symbol>();

        private bool _isAbstract;
        private bool _isStatic;

        private string _name;

        // private string _namespace = string.Empty;
        private NamedTypeSymbol _baseType;
        private Accessibility _declaredAccessibility = Accessibility.Public;
        private ImmutableArray<AttributeData>.Builder _attributes;

        public SynthesizedTypeSymbol(NamespaceOrTypeSymbol container, AquilaCompilation compilation)
        {
            _container = container;
            _compilation = compilation;

            _attributes = ImmutableArray.CreateBuilder<AttributeData>();
        }

        public override int Arity => 0;

        internal override bool HasTypeArgumentsCustomModifiers => false;

        public override ImmutableArray<CustomModifier> GetTypeArgumentCustomModifiers(int ordinal) =>
            GetEmptyTypeArgumentCustomModifiers(ordinal);

        public override Symbol ContainingSymbol => _container;

        internal override ModuleSymbol ContainingModule => _compilation.SourceModule;

        public override Accessibility DeclaredAccessibility => _declaredAccessibility;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get { throw new NotImplementedException(); }
        }

        public override bool IsAbstract => _isAbstract;

        public override bool IsSealed => false;

        public override bool IsStatic => _isStatic;

        public override bool IsSerializable => false;

        public override string Name => _name ??
                                       throw new NullReferenceException(
                                           "For this type the name was not set. Please invoke SetName before get it");

        // public override string NamespaceName => _namespace ??
        //                                         throw new NullReferenceException(
        //                                             "For this type the NamespaceName was not set. Please invoke SetNamespaceName before get it");

        public override NamedTypeSymbol BaseType => _baseType ?? _compilation.CoreTypes.Object;

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

        public override ImmutableArray<AttributeData> GetAttributes()
        {
            return _attributes.ToImmutable();
        }

        public override ImmutableArray<Symbol> GetMembers(string name) =>
            GetMembers().Where(m => m.Name == name).AsImmutable();


        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers() =>
            GetMembers().OfType<NamedTypeSymbol>().AsImmutable();

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name) => GetMembers()
            .OfType<NamedTypeSymbol>().Where(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            .AsImmutable();

        internal override ImmutableArray<NamedTypeSymbol> GetDeclaredInterfaces(ConsList<Symbol> basesBeingResolved) =>
            ImmutableArray<NamedTypeSymbol>.Empty;

        internal override IEnumerable<IFieldSymbol> GetFieldsToEmit() =>
            GetMembers().OfType<FieldSymbol>().AsImmutable();

        internal override IEnumerable<IMethodSymbol> GetMethodsToEmit()
        {
            return GetMembers().OfType<MethodSymbol>().AsImmutable();
        }

        internal override ImmutableArray<NamedTypeSymbol> GetInterfacesToEmit() =>
            ImmutableArray<NamedTypeSymbol>.Empty;

        public override ImmutableArray<MethodSymbol> StaticConstructors => ImmutableArray<MethodSymbol>.Empty;

        public void AddMember(Symbol symbol)
        {
            if (_lazyMembers == null)
            {
                Interlocked.CompareExchange(ref _lazyMembers, new ConcurrentBag<Symbol>(), null);
            }

            _lazyMembers.Add(symbol);
        }

        #region Setters

        public SynthesizedTypeSymbol SetIsAbstract(bool value)
        {
            _isAbstract = value;
            return this;
        }

        public SynthesizedTypeSymbol SetIsStatic(bool value)
        {
            _isStatic = value;
            return this;
        }

        public SynthesizedTypeSymbol SetName(string value)
        {
            _name = value;
            return this;
        }

        // public SynthesizedTypeSymbol SetNamespace(string value)
        // {
        //     _namespace = value;
        //     return this;
        // }

        public SynthesizedTypeSymbol SetBaseType(INamedTypeSymbol value)
        {
            return SetBaseType((NamedTypeSymbol)value);
        }

        public SynthesizedTypeSymbol SetBaseType(NamedTypeSymbol value)
        {
            _baseType = value;
            return this;
        }

        public SynthesizedTypeSymbol SetAccess(Accessibility accessibility)
        {
            _declaredAccessibility = accessibility;
            return this;
        }

        public SynthesizedTypeSymbol AddAttribute(AttributeData attribute)
        {
            _attributes.Add(attribute);
            return this;
        }

        #endregion
    }
}