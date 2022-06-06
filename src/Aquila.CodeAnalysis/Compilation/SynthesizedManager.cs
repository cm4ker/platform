using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.Symbols;
using Roslyn.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Cci = Microsoft.Cci;


namespace Aquila.CodeAnalysis.Emit
{
    /// <summary>
    /// Manages synthesized symbols in the module builder.
    /// </summary>
    internal class SynthesizedManager
    {
        readonly PEModuleBuilder _module;

        private List<SynthesizedNamespaceSymbol> _namespaces = new List<SynthesizedNamespaceSymbol>();

        public AquilaCompilation DeclaringCompilation => _module.Compilation;

        readonly ConcurrentDictionary<Cci.ITypeDefinition, List<Symbol>> _membersByType =
            new ConcurrentDictionary<Cci.ITypeDefinition, List<Aquila.CodeAnalysis.Symbols.Symbol>>();

        public SynthesizedManager(PEModuleBuilder module)
        {
            Contract.ThrowIfNull(module);

            _module = module;
        }

        public ImmutableArray<NamespaceSymbol> Namespaces =>
            _namespaces.ToImmutableArray().CastArray<NamespaceSymbol>();

        #region Synthesized Members

        public SynthesizedTypeSymbol SynthesizeType(NamespaceOrTypeSymbol container, string name)
        {
            var type = new SynthesizedTypeSymbol(container, DeclaringCompilation);
            type.SetName(name);

            if (container is SynthesizedNamespaceSymbol ns)
            {
                ns.AddType(type);
            }

            return type;
        }

        public SynthesizedNamespaceSymbol SynthesizeNamespace(INamespaceSymbol container, string name)
        {
            var ns = new SynthesizedNamespaceSymbol(container, name);
            _namespaces.Add(ns);
            return ns;
        }

        public SynthesizedMethodSymbol SynthesizeMethod(NamedTypeSymbol container)
        {
            var method = new SynthesizedMethodSymbol(container);
            return method;
        }

        public SynthesizedCtorSymbol SynthesizeConstructor(NamedTypeSymbol container)
        {
            var ctor = new SynthesizedCtorSymbol(container);
            return ctor;
        }

        /// <summary>
        /// Gets synthezised members contained in <paramref name="container"/>.
        /// </summary>
        /// <remarks>
        /// This method is not thread-safe, it is expected to be called after all
        /// the synthesized members were added to <paramref name="container"/>.
        /// </remarks>
        /// <typeparam name="T">Type of members to enumerate.</typeparam>
        /// <param name="container">Containing type.</param>
        /// <returns>Enumeration of synthesized type members.</returns>
        public IEnumerable<T> GetMembers<T>(Cci.ITypeDefinition container) where T : ISymbol
        {
            List<Aquila.CodeAnalysis.Symbols.Symbol> list;
            if (_membersByType.TryGetValue(container, out list) && list.Count != 0)
            {
                return list.OfType<T>();
            }
            else
            {
                return SpecializedCollections.EmptyEnumerable<T>();
            }
        }

        #endregion
    }
}