using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Symbols.Source
{
    /// <summary>
    /// Describes set of union types
    /// </summary>
    internal sealed class SourceUnionTypeSymbol : TypeSymbol
    {
        public SourceUnionTypeSymbol()
        {
        }

        internal override ObsoleteAttributeData ObsoleteAttributeData { get; }
        public override SymbolKind Kind { get; }
        public override Symbol ContainingSymbol { get; }
        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences { get; }
        public override Accessibility DeclaredAccessibility { get; }
        public override bool IsStatic { get; }
        public override bool IsAbstract { get; }
        public override bool IsSealed { get; }

        public override ImmutableArray<Symbol> GetMembers()
        {
            throw new System.NotImplementedException();
        }

        public override ImmutableArray<Symbol> GetMembers(string name)
        {
            throw new System.NotImplementedException();
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
        {
            throw new System.NotImplementedException();
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
        {
            throw new System.NotImplementedException();
        }

        public override TypeKind TypeKind { get; }
    }
}