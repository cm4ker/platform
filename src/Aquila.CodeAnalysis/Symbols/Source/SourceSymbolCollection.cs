using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Aquila.CodeAnalysis.Syntax;
using Aquila.Compiler.Utilities;
using Aquila.Syntax;
using Aquila.Syntax.Syntax;
using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Utilities;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Symbols.Source
{
    /// <summary>
    /// Collection of source symbols.
    /// </summary>
    internal class SourceSymbolCollection
    {
        #region SymbolsCache

        sealed class SymbolsCache<TKey, TSymbol>
        {
            int _cacheVersion = -1;
            MultiDictionary<TKey, TSymbol> _cacheDict;
            List<TSymbol> _cacheAll;

            readonly SourceSymbolCollection _table;

            // readonly Func<SourceFileSymbol, IEnumerable<TSymbol>> _getter;
            readonly Func<TSymbol, TKey> _key;
            readonly Func<TSymbol, bool> _isVisible;

            public SymbolsCache(SourceSymbolCollection table,
                // Func<SourceFileSymbol, IEnumerable<TSymbol>> getter,
                Func<TSymbol, TKey> key,
                Func<TSymbol, bool> isVisible)
            {
                _table = table;
                // _getter = getter;
                _key = key;
                _isVisible = isVisible;
            }

            void EnsureUpdated()
            {
                if (_table._version != _cacheVersion)
                {
                    _cacheAll = new List<TSymbol>();
                    _cacheDict = new MultiDictionary<TKey, TSymbol>();

                    // foreach (var f in _table._files.Values)
                    // {
                    //     var symbols = _getter(f);
                    //     _cacheAll.AddRange(symbols);
                    //
                    //     foreach (var s in symbols)
                    //     {
                    //         // add all symbols,
                    //         // _isVisible may have side effects accessing this incomplete collection
                    //         _cacheDict.Add(_key(s), s);
                    //     }
                    // }

                    _cacheVersion = _table._version;
                }
            }

            /// <summary>
            /// All symbols, both visible and not visible.
            /// </summary>
            public IEnumerable<TSymbol> Symbols
            {
                get
                {
                    EnsureUpdated();
                    return _cacheAll;
                }
            }

            public IEnumerable<TSymbol> GetAll(TKey key)
            {
                EnsureUpdated();
                return _cacheDict[key];
            }

            /// <summary>
            /// Gets all visible symbols.
            /// </summary>
            public IEnumerable<TSymbol> this[TKey key]
            {
                get { return GetAll(key).Where(_isVisible); }
            }
        }

        #endregion

        /// <summary>
        /// Collection version, increased when a syntax tree is added or removed.
        /// </summary>
        public int Version => _version;

        int
            _version = 0;

        /// <summary>
        /// Gets reference to containing compilation object.
        /// </summary>
        public AquilaCompilation Compilation => _compilation;

        readonly AquilaCompilation _compilation;

        readonly Dictionary<SyntaxTree, int> _ordinalMap = new Dictionary<SyntaxTree, int>();
        private readonly List<AquilaSyntaxTree> _trees = new List<AquilaSyntaxTree>();

        private MultiDictionary<QualifiedName, SourceMethodSymbol> _extendMethods;
        private List<SourceMethodSymbol> _globalMethods;

        /// <summary>
        /// Class holding app-static constants defined in compile-time.
        /// <code>static class &lt;constants&gt; { ... }</code>
        /// </summary>
        internal SynthesizedTypeSymbol DefinedConstantsContainer { get; }


        public IDictionary<SyntaxTree, int> OrdinalMap => _ordinalMap;

        public SourceSymbolCollection(AquilaCompilation compilation)
        {
            Contract.ThrowIfNull(compilation);
            _compilation = compilation;

            // _methods = new SymbolsCache<QualifiedName, MethodSymbol>(this, f => f.,
            //     f => f.QUali, f => !f.IsConditional);

            _extendMethods = new MultiDictionary<QualifiedName, SourceMethodSymbol>();
            _globalMethods = new List<SourceMethodSymbol>();

            // class <constants> { ... }

            this.DefinedConstantsContainer =
                _compilation.AnonymousTypeManager.SynthesizeType("<Constants>", true);

            PopulateDefinedConstants(DefinedConstantsContainer, _compilation.Options.Defines);
        }

        void PopulateDefinedConstants(SynthesizedTypeSymbol container,
            ImmutableDictionary<string, string> defines)
        {
            if (defines == null || defines.IsEmpty)
            {
                return;
            }

            foreach (var d in defines)
            {
                // resolve the value from string
                ConstantValue value;

                if (string.IsNullOrEmpty(d.Value))
                {
                    value = ConstantValue.True;
                }
                else if (string.Equals(d.Value, "null", StringComparison.OrdinalIgnoreCase))
                {
                    value = ConstantValue.Null;
                }
                else if (long.TryParse(d.Value, out var l))
                {
                    value = (l >= int.MinValue && l <= int.MaxValue)
                        ? ConstantValue.Create((int)l)
                        : ConstantValue.Create(l);
                }
                else if (bool.TryParse(d.Value, out var b))
                {
                    value = ConstantValue.Create(b);
                }
                else if (double.TryParse(d.Value, System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out var f))
                {
                    value = ConstantValue.Create(f);
                }
                else if (d.Value.Length >= 2 && d.Value[0] == '\"' && d.Value[d.Value.Length - 1] == d.Value[0])
                {
                    value = ConstantValue.Create(d.Value.Substring(1, d.Value.Length - 2));
                }
                else
                {
                    value = ConstantValue.Create(d.Value);
                }

                // TODO: expressions ? app constants can be also static properties
                // TODO: check name

                var type = _compilation.GetSpecialType(value.IsNull ? SpecialType.System_Object : value.SpecialType);

                //
                container.AddMember(
                    new SynthesizedFieldSymbol(container)
                        .SetType(type)
                        .SetName(d.Key)
                        .SetAccess(Accessibility.Public)
                        .SetConstant(value)
                );
            }
        }

        public void AddSyntaxTreeRange(IEnumerable<AquilaSyntaxTree> trees)
        {
            trees.ForEach(AddSyntaxTree);
        }

        public void AddSyntaxTree(AquilaSyntaxTree tree)
        {
            Contract.ThrowIfNull(tree);

            Debug.Assert(tree.Source != null);

            foreach (var f in tree.Functions)
            {
                var method = new SourceGlobalMethodSymbol(DefinedConstantsContainer, f);

                _globalMethods.Add(method);
                DefinedConstantsContainer.AddMember(method);
            }

            //Go through components and extends
            foreach (var com in tree.Components)
            {
                foreach (var ex in com.Extends)
                {
                    foreach (var mDecl in ex.Methods)
                    {
                        //Full path to the Method is Component.Extend.Method

                        var qn = QualifiedName.Parse($"{com.Identifier.Text}.{ex.Identifier.Text}", false);

                        //Try to find type
                        var type = Compilation.PlatformSymbolCollection.GetType(qn);

                        if (type is SynthesizedTypeSymbol sts)
                        {
                            var m = new SourceMethodSymbol(sts, mDecl);

                            _extendMethods.Add(qn, m);
                            sts.AddMember(m);
                        }
                    }
                }
            }

            _ordinalMap.Add(tree, _ordinalMap.Count);
            _trees.Add(tree);

            _version++;
        }

        public bool RemoveSyntaxTree(string fname)
        {
            var relative = AquilaFileUtilities.GetRelativePath(fname, _compilation.Options.BaseDirectory);
            // if (_files.Remove(relative))
            // {
            //     _version++;
            //
            //     return true;
            // }

            return false;
        }

        /// <summary>
        /// Gets compilation syntax trees.
        /// </summary>
        public IEnumerable<AquilaSyntaxTree> SyntaxTrees => _trees;

        public IEnumerable<MethodSymbol> GetMethods()
        {
            foreach (var g in _globalMethods)
            {
                yield return g;
            }

            foreach (var em in _extendMethods)
            {
                foreach (var em_ov in em.Value)
                {
                    yield return em_ov;
                }
            }
        }

        /// <summary>
        /// Gets enumeration of all methods (global code, functions, lambdas and class methods) in source code.
        /// </summary>
        public IEnumerable<SourceMethodSymbol> AllMethods // all functions + global code + methods + lambdas
        {
            get
            {
                var methods = GetMethods().Cast<SourceMethodSymbol>();
                return methods;
            }
        }

        public NamedTypeSymbol GetType(QualifiedName name, Dictionary<QualifiedName, INamedTypeSymbol> resolved = null)
        {
            NamedTypeSymbol first = null;
            List<NamedTypeSymbol> alternatives = null;

            return new MissingMetadataTypeSymbol(name.ClrName(), 0, false);
        }
    }
}