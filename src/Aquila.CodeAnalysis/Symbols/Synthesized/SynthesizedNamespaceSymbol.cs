using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Aquila.CodeAnalysis.Symbols.PE;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.PooledObjects;
using MoreLinq;

namespace Aquila.CodeAnalysis.Symbols.Synthesized
{
    sealed class SynthesizedNamespaceSymbol : NamespaceSymbol
    {
        private readonly NamespaceSymbol _container;
        private readonly string _name;

        /// <summary>
        /// A map of types immediately contained within this namespace 
        /// grouped by their name (case-sensitively).
        /// </summary>
        List<NamedTypeSymbol> _types;


        public SynthesizedNamespaceSymbol(INamespaceSymbol container, string name)
        {
            _container = (NamespaceSymbol)container;
            _name = name;
        }

        public override string Name => _name;

        public override Symbol ContainingSymbol => _container;
        public override AssemblySymbol ContainingAssembly => _container.ContainingAssembly;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences =>
            ImmutableArray<SyntaxReference>.Empty;


        public override ImmutableArray<Symbol> GetMembers()
        {
            var builder = ArrayBuilder<Symbol>.GetInstance(_types.Count);
            builder.AddRange(_types);
            return builder.ToImmutableAndFree();
        }

        public override ImmutableArray<Symbol> GetMembers(string name)
        {
            return _types.Where(x => x.Name == name).OfType<Symbol>().ToImmutableArray();
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
        {
            return _types.ToImmutableArray();
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
        {
            return GetTypeMembers().WhereAsArray(x => x.Name == name);
        }
    }
}