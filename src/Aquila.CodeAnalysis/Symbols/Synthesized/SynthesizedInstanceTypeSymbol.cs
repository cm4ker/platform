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
    /// Synthesized non-generic static class.
    /// class { ... }
    /// </summary>
    class SynthesizedInstanceTypeSymbol : NamedTypeSymbol
    {
        readonly PhpCompilation _compilation;
        private NamedTypeSymbol _baseType;
        readonly NamedTypeSymbol _containingType;

        ConcurrentBag<Symbol> _lazyMembers;

        public void AddMember(Symbol symbol)
        {
            if (_lazyMembers == null)
            {
                Interlocked.CompareExchange(ref _lazyMembers, new ConcurrentBag<Symbol>(), null);
            }

            _lazyMembers.Add(symbol);
        }

        public SynthesizedInstanceTypeSymbol(PhpCompilation compilation, string name, NamedTypeSymbol baseType = null,
            NamedTypeSymbol containingType = null, Accessibility accessibility = Accessibility.Internal)
        {
            _compilation = compilation;
            _baseType = baseType ?? _compilation.CoreTypes.Object;
            ;

            _containingType = containingType;

            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.DeclaredAccessibility = accessibility;
        }

        public override int Arity => 0;

        internal override bool HasTypeArgumentsCustomModifiers => false;

        public override ImmutableArray<CustomModifier> GetTypeArgumentCustomModifiers(int ordinal) =>
            GetEmptyTypeArgumentCustomModifiers(ordinal);

        public override Symbol ContainingSymbol => (Symbol) _containingType ?? _compilation.SourceModule;

        internal override IModuleSymbol ContainingModule => _compilation.SourceModule;

        public override Accessibility DeclaredAccessibility { get; }

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get { throw new NotImplementedException(); }
        }

        public override bool IsAbstract => false;

        public override bool IsSealed => false;

        public override bool IsStatic => false;

        public override bool IsSerializable => false;

        public override string Name { get; }

        public override string NamespaceName => string.Empty;

        public override NamedTypeSymbol BaseType => _baseType;

        public override TypeKind TypeKind => TypeKind.Class;

        internal override bool IsInterface => false;

        internal override bool IsWindowsRuntimeImport => false;

        public override bool IsImplicitlyDeclared => true;

        public override bool IsImplicitClass => true;

        internal override TypeLayout Layout => default;

        internal override bool MangleName => false;

        internal override ObsoleteAttributeData ObsoleteAttributeData => null;

        internal override bool ShouldAddWinRTMembers => false;

        public override ImmutableArray<Symbol> GetMembers() =>
            _lazyMembers != null ? _lazyMembers.AsImmutable() : ImmutableArray<Symbol>.Empty;

        public override ImmutableArray<Symbol> GetMembers(string name) => _lazyMembers != null
            ? _lazyMembers.Where(s => s.Name == name).AsImmutable()
            : ImmutableArray<Symbol>.Empty;

        public override ImmutableArray<Symbol> GetMembersByPhpName(string name) => _lazyMembers != null
            ? _lazyMembers.Where(s => string.Equals(s.PhpName(), name, StringComparison.InvariantCultureIgnoreCase))
                .AsImmutable()
            : ImmutableArray<Symbol>.Empty;

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers() => ImmutableArray<NamedTypeSymbol>.Empty;

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name) =>
            ImmutableArray<NamedTypeSymbol>.Empty;

        internal override ImmutableArray<NamedTypeSymbol> GetDeclaredInterfaces(ConsList<Symbol> basesBeingResolved) =>
            ImmutableArray<NamedTypeSymbol>.Empty;

        internal override IEnumerable<IFieldSymbol> GetFieldsToEmit() => _lazyMembers != null
            ? _lazyMembers.OfType<IFieldSymbol>().AsImmutable()
            : ImmutableArray<IFieldSymbol>.Empty;

        internal override ImmutableArray<NamedTypeSymbol> GetInterfacesToEmit() =>
            ImmutableArray<NamedTypeSymbol>.Empty;

        public override ImmutableArray<MethodSymbol> StaticConstructors => ImmutableArray<MethodSymbol>.Empty;
    }
}