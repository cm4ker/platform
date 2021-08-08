using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
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
        Dictionary<string, NamedTypeSymbol> _types;


        public SynthesizedNamespaceSymbol(INamespaceSymbol container, string name)
        {
            _container = (NamespaceSymbol)container;
            _name = name;

            _types = new Dictionary<string, NamedTypeSymbol>();
        }

        internal override AquilaCompilation DeclaringCompilation => _container.DeclaringCompilation;

        public void AddType(NamedTypeSymbol type)
        {
            Debug.Assert(type.ContainingNamespace == this);
            _types.Add(type.Name, type);
        }

        public void AddAlias(string alias, NamedTypeSymbol type)
        {
            _types.Add(alias, type);
        }

        public override string Name => _name;

        public override Symbol ContainingSymbol => _container;
        public override AssemblySymbol ContainingAssembly => _container.ContainingAssembly;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences =>
            ImmutableArray<SyntaxReference>.Empty;


        public override ImmutableArray<Symbol> GetMembers()
        {
            var builder = ArrayBuilder<Symbol>.GetInstance(_types.Count);
            builder.AddRange(_types.Values.Distinct());

            return builder.ToImmutableAndFree();
        }

        public override ImmutableArray<Symbol> GetMembers(string name)
        {
            if (_types.TryGetValue(name, out var type))

                return new List<Symbol>() { type }.ToImmutableArray();

            return ImmutableArray<Symbol>.Empty;
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
        {
            return GetMembers().OfType<NamedTypeSymbol>().ToImmutableArray();
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
        {
            return GetMembers(name).OfType<NamedTypeSymbol>().ToImmutableArray();
        }
    }
}