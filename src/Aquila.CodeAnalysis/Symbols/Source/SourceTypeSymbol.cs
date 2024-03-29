﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using Aquila.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Aquila.CodeAnalysis.Syntax;
using Aquila.Syntax.Declarations;
using Microsoft.CodeAnalysis;
using Roslyn.Utilities;
using TypeLayout = Microsoft.CodeAnalysis.TypeLayout;

namespace Aquila.CodeAnalysis.Symbols.Source
{
    internal class SourceTypeSymbol : NamedTypeSymbol
    {
        private readonly NamespaceOrTypeSymbol _container;
        private readonly MergedTypeDecl _typeSyntax;
        private ImmutableArray<Symbol> _members;

        public SourceTypeSymbol(NamespaceOrTypeSymbol container, MergedTypeDecl typeSyntax)
        {
            _container = container;
            _typeSyntax = typeSyntax;
        }

        internal override ObsoleteAttributeData ObsoleteAttributeData { get; }
        public override Symbol ContainingSymbol => _container;
        public override NamedTypeSymbol BaseType => DeclaringCompilation.GetSpecialType(SpecialType.System_Object);

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences { get; }
        public override Accessibility DeclaredAccessibility => Accessibility.Internal;
        public override bool IsStatic => false;

        public override ImmutableArray<CustomModifier> GetTypeArgumentCustomModifiers(int ordinal)
        {
            return ImmutableArray<CustomModifier>.Empty;
        }

        public override int Arity => 0;
        public override bool IsSerializable => true;
        internal override bool MangleName => false;


        public override string Name => _typeSyntax.Name;

        internal override ImmutableArray<NamedTypeSymbol> GetDeclaredInterfaces(ConsList<Symbol> basesBeingResolved)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        internal override bool IsInterface => false;
        internal override bool HasTypeArgumentsCustomModifiers => false;

        internal override IEnumerable<IFieldSymbol> GetFieldsToEmit()
        {
            return GetMembers().OfType<IFieldSymbol>();
        }

        internal override ImmutableArray<NamedTypeSymbol> GetInterfacesToEmit()
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        public override bool IsAbstract => false;
        internal override bool IsWindowsRuntimeImport => false;
        internal override bool ShouldAddWinRTMembers => false;
        public override bool IsSealed => true;
        internal override TypeLayout Layout => default(TypeLayout);

        public override ImmutableArray<Symbol> GetMembers()
        {
            CoreEnsureMembers();
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

        private void CoreEnsureMembers()
        {
            if (_members == null || _members.IsDefaultOrEmpty)
            {
                var builder = ImmutableArray.CreateBuilder<Symbol>();
                builder.AddRange(_typeSyntax.TypeDecl.Fields.Select(x => (Symbol)new SourceFieldSymbol(x, this)));
                builder.AddRange(_typeSyntax.FuncDecls.Select(x => new SourceMethodSymbol(this, x)));

                SourceTypeSymbolHelper.AddDefaultInstanceTypeSymbolMembers(this, builder);
                _members = builder.ToImmutable();
            }
        }

        public override TypeKind TypeKind => TypeKind.Class;

        public void GetDiagnostics(DiagnosticBag diagnostic)
        {
        }
    }
}