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
            EnsureLazyNamespaces();
            var ns = new SynthesizedNamespaceSymbol(container, name);
            _lazySynthesizedNamespaces.Add(ns);

            return ns;
        }

        public SynthesizedTypeSymbol SynthesizeType(NamespaceOrTypeSymbol container)
        {
            var type = new SynthesizedTypeSymbol(container, Compilation);

            EnsureLazyTypes();

            _lazySynthesizedTypes.Add(type);

            return type;
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

        private void EnsureLazyTypes()
        {
            if (_lazySynthesizedTypes == null)
            {
                Interlocked.CompareExchange(ref _lazySynthesizedTypes, new ConcurrentBag<SynthesizedTypeSymbol>(),
                    null);

                EnsureMetadataPopulated();
            }
        }

        private void EnsureLazyNamespaces()
        {
            if (_lazySynthesizedNamespaces == null)
            {
                Interlocked.CompareExchange(ref _lazySynthesizedNamespaces,
                    new ConcurrentBag<SynthesizedNamespaceSymbol>(),
                    null);
            }
        }

        private void EnsureMetadataPopulated()
        {
            AddMetadata(Compilation.MetadataCollection.GetSemanticMetadata());
        }

        #endregion

        internal NamedTypeSymbol GetType(QualifiedName name,
            Dictionary<QualifiedName, INamedTypeSymbol> resolved = null)
        {
            EnsureLazyTypes();

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
            EnsureLazyNamespaces();
            return _lazySynthesizedNamespaces.FirstOrDefault(x => x.Name == name);
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
            EnsureLazyTypes();
            return _lazySynthesizedTypes;
        }

        public IEnumerable<SynthesizedTypeSymbol> SynthesizedTypes =>
            GetAllCreatedTypes().OfType<SynthesizedTypeSymbol>();

        private void AddMetadata(IEnumerable<SMEntity> entityMetadata)
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