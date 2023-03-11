using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Aquila.CodeAnalysis.Emit;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Aquila.Compiler.Utilities;
using Aquila.Metadata;
using Aquila.Syntax.Metadata;
using Microsoft.CodeAnalysis;
using MetadataSymbolProvider = Aquila.CodeAnalysis.Metadata.MetadataSymbolProvider;

namespace Aquila.CodeAnalysis.Public
{
    /// <summary>
    /// TODO: replace this by <see cref="SynthesizedManager"/> from PEModuleBuilder.
    /// Not construct entire platform symbols on creating compilation. Construct it on building process and lazy
    /// </summary>
    internal class PlatformSymbolCollection
    {
        private int instanceNumber = 1;


        public PlatformSymbolCollection(AquilaCompilation compilation)
        {
            Compilation = compilation;
        }

        /// <summary> 
        /// Current compilation.
        /// </summary>
        public AquilaCompilation Compilation { get; }

        #region Types

        private ConcurrentBag<SynthesizedTypeSymbol> _lazySynthesizedTypes;
        private ConcurrentBag<SynthesizedNamespaceSymbol> _lazySynthesizedNamespaces;

        public SynthesizedNamespaceSymbol SynthesizeNamespace(INamespaceSymbol container, string name)
        {
            EnsureMetadataPopulated();
            var ns = new SynthesizedNamespaceSymbol(container, name);
            _lazySynthesizedNamespaces.Add(ns);

            return ns;
        }

        public SynthesizedTypeSymbol SynthesizeType(NamespaceOrTypeSymbol container, string name)
        {
            var type = new SynthesizedTypeSymbol(container, Compilation);
            type.SetName(name);

            if (container is SynthesizedNamespaceSymbol ns)
            {
                ns.AddType(type);
            }

            EnsureMetadataPopulated();

            _lazySynthesizedTypes.Add(type);

            return type;
        }

        public SynthesizedUnionTypeSymbol SynthesizeUnionType(NamespaceOrTypeSymbol container,
            IEnumerable<TypeSymbol> types)
        {
            EnsureMetadataPopulated();

            var symbol = _lazySynthesizedTypes.OfType<SynthesizedUnionTypeSymbol>()
                .FirstOrDefault(x =>
                    x.ContainingTypes
                        .OrderBy(x => x.Name)
                        .ThenBy(x => x.SpecialType)
                        .SequenceEqual(
                            types.OrderBy(x => x.Name)
                                .ThenBy(x => x.SpecialType), (typeSymbol, symbol1) =>
                                typeSymbol.Equals(symbol1)));

            if (symbol != null) return symbol;

            var type = new SynthesizedUnionTypeSymbol(container, types);

            if (container is SynthesizedNamespaceSymbol ns)
            {
                ns.AddType(type);
            }

            _lazySynthesizedTypes.Add(type);

            return type;
        }

        public void RegisterAlias(string alias, SynthesizedTypeSymbol type, SynthesizedNamespaceSymbol ns)
        {
            ns.AddAlias(alias, type);
        }

        public SynthesizedFieldSymbol SynthesizeField(NamedTypeSymbol container)
        {
            var field = new SynthesizedFieldSymbol(container);
            return field;
        }

        public SynthesizedPropertySymbol SynthesizeProperty(NamedTypeSymbol container)
        {
            var prop = new SynthesizedPropertySymbol(container);
            return prop;
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

        private object _lock = new object();

        private void EnsureMetadataPopulated()
        {
            //TODO: remove lock and rewrite it like get collections with types manner
            lock (_lock)
                if (_lazySynthesizedTypes == null && _lazySynthesizedNamespaces == null)
                {
                    var types = new ConcurrentBag<SynthesizedTypeSymbol>();
                    var namespaces = new ConcurrentBag<SynthesizedNamespaceSymbol>();

                    Interlocked.CompareExchange(ref _lazySynthesizedTypes, types, null);
                    Interlocked.CompareExchange(ref _lazySynthesizedNamespaces, namespaces, null);

                    PopulateMetadata(Compilation.MetadataProvider.GetSemanticMetadata());
                }
        }

        #endregion

        internal NamedTypeSymbol TryGetType(QualifiedName name,
            Dictionary<QualifiedName, INamedTypeSymbol> resolved = null)
        {
            EnsureMetadataPopulated();

            NamedTypeSymbol first = null;
            List<NamedTypeSymbol> alternatives = null;

            var types = _lazySynthesizedTypes.Where(x => x.MakeQualifiedName() == name);
            foreach (var t in types)
            {
                if (first == null)
                {
                    first = t;
                    alternatives = new List<NamedTypeSymbol>() { first };
                }
                else
                {
                    alternatives.Add(t);
                }
            }

            var result = alternatives?.Count == 1 ? first : null;

            if (resolved != null)
            {
                resolved[name] = result;
            }

            return result;
        }

        internal NamespaceSymbol GetNamespace(string name)
        {
            return GetNamespaces().FirstOrDefault(x => x.Name == name);
        }

        internal ImmutableArray<NamespaceSymbol> GetNamespaces()
        {
            EnsureMetadataPopulated();
            return _lazySynthesizedNamespaces.OfType<NamespaceSymbol>().ToImmutableArray();
        }

        /// <summary>
        /// Returns null if synthesized type in platform manager not found
        /// </summary>
        /// <param name="name"></param>
        /// <param name="resolved"></param>
        /// <returns></returns>
        internal SynthesizedTypeSymbol GetSynthesizedType(QualifiedName name,
            Dictionary<QualifiedName, INamedTypeSymbol> resolved = null)
        {
            var type = TryGetType(name, resolved);

            if (type is SynthesizedTypeSymbol st) return st;

            return null;
        }

        internal IEnumerable<NamedTypeSymbol> GetAllCreatedTypes()
        {
            EnsureMetadataPopulated();
            return _lazySynthesizedTypes;
        }

        public IEnumerable<SynthesizedTypeSymbol> SynthesizedTypes =>
            GetAllCreatedTypes().OfType<SynthesizedTypeSymbol>();

        private void PopulateMetadata(IEnumerable<SMEntity> entityMetadata)
        {
            MetadataSymbolProvider mp = new MetadataSymbolProvider(Compilation);

            var smEntities = entityMetadata as SMEntity[] ?? entityMetadata.ToArray();

            mp.PopulateNamespaces(smEntities);
            mp.PopulateTypes(smEntities);
            mp.PopulateMembers(smEntities);
        }
    }
}