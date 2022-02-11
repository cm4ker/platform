using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Aquila.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Source;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Aquila.CodeAnalysis.Syntax;
using Aquila.CodeAnalysis.Utilities;
using Aquila.Compiler.Utilities;
using Aquila.Metadata;
using Aquila.Syntax.Metadata;
using Aquila.Syntax.Parser;
using Aquila.Syntax.Syntax;
using Microsoft.Cci;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Public
{
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
                    x.ContainingTypes.OrderBy(x => x.Name).ThenBy(x => x.SpecialType)
                        .SequenceEqual(types.OrderBy(x => x.Name).ThenBy(x => x.SpecialType)));

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
            var prop = new SynthesizedMethodSymbol(container);
            return prop;
        }

        public SynthesizedCtorSymbol SynthesizeConstructor(NamedTypeSymbol container)
        {
            var prop = new SynthesizedCtorSymbol(container);
            return prop;
        }

        private object _lock = new object();

        private void EnsureMetadataPopulated()
        {
            //Lock metadata caching because it 
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

        internal NamedTypeSymbol GetType(QualifiedName name,
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
                }
                else
                {
                    // ambiguity
                    if (alternatives == null)
                    {
                        alternatives = new List<NamedTypeSymbol>() { first };
                    }

                    alternatives.Add(t);
                }
            }

            var result =
                (alternatives != null)
                    ? new AmbiguousErrorTypeSymbol(alternatives.AsImmutable()) // ambiguity
                    : first ?? new MissingMetadataTypeSymbol(name.ClrName(), 0, false);

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
            var type = GetType(name, resolved);

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

    internal static class SynthExtensions
    {
        public static void CreatePropertyWithBackingField(this PlatformSymbolCollection ps, SynthesizedTypeSymbol type,
            TypeSymbol propType, string name)
        {
            var backingField = ps.SynthesizeField(type)
                .SetName($"<{name}>k_BackingField")
                .SetAccess(Accessibility.Private)
                .SetType(propType);

            //args and fields
            var field = new FieldPlace(backingField);
            var thisArg = new ArgPlace(type, 0);

            var getter = ps.SynthesizeMethod(type)
                .SetName($"get_{name}")
                .SetAccess(Accessibility.Public)
                .SetReturn(propType)
                .SetMethodBuilder((m, d) => (il) =>
                {
                    thisArg.EmitLoad(il);
                    field.EmitLoad(il);
                    il.EmitRet(false);
                });

            var setter = ps.SynthesizeMethod(type)
                .SetName($"set_{name}")
                .SetAccess(Accessibility.Public);

            var param = new SynthesizedParameterSymbol(setter, propType, 0, RefKind.None);
            setter.SetParameters(param);

            var paramPlace = new ParamPlace(param);


            setter
                .SetMethodBuilder((m, d) => (il) =>
                {
                    thisArg.EmitLoad(il);
                    paramPlace.EmitLoad(il);

                    field.EmitStore(il);
                    il.EmitRet(true);
                });


            var peProp = ps.SynthesizeProperty(type);
            peProp
                .SetName(name)
                .SetGetMethod(getter)
                .SetSetMethod(setter)
                .SetType(propType);

            type.AddMember(backingField);
            type.AddMember(getter);
            type.AddMember(setter);
            type.AddMember(peProp);
        }
    }
}