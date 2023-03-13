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
        private readonly List<SynthesizedNamespaceSymbol> _namespaces = new();
        private readonly PEModuleBuilder _module;
        private readonly ConcurrentDictionary<Cci.ITypeDefinition, List<Symbol>> _membersByType = new();

        public SynthesizedManager(PEModuleBuilder module)
        {
            Contract.ThrowIfNull(module);

            _module = module;
        }

        public AquilaCompilation DeclaringCompilation => _module.Compilation;

        public ImmutableArray<NamespaceSymbol> Namespaces =>
            _namespaces.ToImmutableArray().CastArray<NamespaceSymbol>();

        #region Synthesized Members

        public SynthesizedTypeSymbol SynthesizeType(NamespaceOrTypeSymbol container, string name)
        {
            var type = new SynthesizedTypeSymbol(container, DeclaringCompilation);
            type.SetName(name);

            switch (container)
            {
                case SynthesizedNamespaceSymbol ns:
                    ns.AddType(type);
                    break;
                case NamedTypeSymbol typeContainer:
                    AddMemberCore(typeContainer, type);
                    break;
                default:
                    throw new InvalidOperationException();
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
            return AddMemberCore(container, new SynthesizedMethodSymbol(container));
        }

        public SynthesizedCtorSymbol SynthesizeConstructor(NamedTypeSymbol container)
        {
            return AddMemberCore(container, new SynthesizedCtorSymbol(container));
        }

        public SynthesizedFieldSymbol SynthesizeField(NamedTypeSymbol container)
        {
            return AddMemberCore(container, new SynthesizedFieldSymbol(container));
        }

        public void AddMembers(NamedTypeSymbol container, IEnumerable<Symbol> members)
        {
            foreach (var member in members)
            {
                if (member is not (SynthesizedMethodSymbol or SynthesizedCtorSymbol or SynthesizedFieldSymbol
                    or SynthesizedTypeSymbol))
                {
                    throw new InvalidOperationException();
                }

                AddMemberCore(container, member);
            }
        }

        public T GetOrCreate<T>(NamedTypeSymbol container, string name) where T : Symbol
        {
            Symbol Factory()
            {
                return typeof(T) switch
                {
                    var t when t == typeof(SynthesizedMethodSymbol) => this.SynthesizeMethod(container).SetName(name),
                    var t when t == typeof(SynthesizedCtorSymbol) => this.SynthesizeConstructor(container),
                    var t when t == typeof(SynthesizedFieldSymbol) => this.SynthesizeField(container).SetName(name),
                    var t when t == typeof(SynthesizedTypeSymbol) => this.SynthesizeType(container, name),
                    _ => throw new InvalidOperationException()
                };
            }

            var list = EnsureList(container);
            var member = list.OfType<T>().FirstOrDefault(x => x.Name == name);
            return member ?? (T)Factory();
        }

        private List<Symbol> EnsureList(Cci.ITypeDefinition type)
        {
            return _membersByType.GetOrAdd(type, (_) => new List<Symbol>());
        }

        private T AddMemberCore<T>(Cci.ITypeDefinition container, T member) where T : Symbol
        {
            var list = EnsureList(container);
            list.Add(member);

            return member;
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
            if (_membersByType.TryGetValue(container, out var list) && list.Count != 0)
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