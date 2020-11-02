using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Symbols.Synthesized
{
    class SynthesizedNamespaceSymbol : NamespaceSymbol
    {
        public override Symbol ContainingSymbol { get; }
        public override AssemblySymbol ContainingAssembly { get; }
        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences { get; }
        public override ImmutableArray<Symbol> GetMembers()
        {
            throw new NotImplementedException();
        }

        public override ImmutableArray<Symbol> GetMembers(string name)
        {
            throw new NotImplementedException();
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
        {
            throw new NotImplementedException();
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
        {
            throw new NotImplementedException();
        }

        public override ImmutableArray<Symbol> GetMembersByPhpName(string name)
        {
            throw new NotImplementedException();
        }
    }
}