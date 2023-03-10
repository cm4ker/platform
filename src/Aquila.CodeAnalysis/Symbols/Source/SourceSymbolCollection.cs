using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Aquila.Compiler.Utilities;
using Aquila.Syntax;
using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Semantics;
using Aquila.Syntax.Declarations;
using Roslyn.Utilities;
using Xunit;

namespace Aquila.CodeAnalysis.Symbols.Source
{
    /// <summary>
    /// Collection of source symbols.
    /// </summary>
    internal class SourceSymbolCollection
    {
        /// <summary>
        /// Gets reference to containing compilation object.
        /// </summary>
        public AquilaCompilation Compilation => _compilation;

        readonly AquilaCompilation _compilation;

        private MergedSourceCode _sourceCode = new MergedSourceCode();


        private List<SourceMethodSymbol> _extendMethods;
        private List<SourceMethodSymbol> _globalMethods;
        private List<NamedTypeSymbol> _types;
        private IEnumerable<AquilaSyntaxTree> _views = ImmutableArray<AquilaSyntaxTree>.Empty;


        /// <summary>
        /// Class holding app-static constants defined in compile-time.
        /// <code>static class &lt;constants&gt; { ... }</code>
        /// </summary>
        internal SynthesizedTypeSymbol DefinedConstantsContainer { get; }

        internal NamespaceOrTypeSymbol SourceTypeContainer { get; set; }

        public IDictionary<SyntaxTree, int> OrdinalMap => _sourceCode.OrdinalMap;

        public SourceSymbolCollection(AquilaCompilation compilation)
        {
            Contract.ThrowIfNull(compilation);
            _compilation = compilation;

            _extendMethods = new List<SourceMethodSymbol>();
            _globalMethods = new List<SourceMethodSymbol>();
            _types = new List<NamedTypeSymbol>();
            
            this.DefinedConstantsContainer =
                _compilation.AnonymousTypeManager.SynthesizeType("<Constants>", true);

            PopulateDefinedConstants(DefinedConstantsContainer, _compilation.Options.Defines);


            SourceTypeContainer = (NamespaceSymbol)_compilation.SourceModule.GlobalNamespace;
        }

        void PopulateDefinedConstants(SynthesizedTypeSymbol container, ImmutableDictionary<string, string> defines)
        {
            if (defines == null || defines.IsEmpty)
            {
            }
        }

        private void UpdateSymbolsCore()
        {
            var modules = _sourceCode.GetModules();

            foreach (var module in modules)
            {
                _types.Add(new SourceModuleTypeSymbol(SourceTypeContainer, module));

                foreach (var function in module.OwnedFunctions)
                {
                    if (function.FuncOwner != null && AstUtils.GetModifiers(function.Modifiers).IsPartial())
                    {
                        Assert.NotNull(function.FuncOwner);

                        var binder = _compilation.GetBinder(function.Parent);
                        var type = binder.TryResolveTypeSymbol(function.FuncOwner.OwnerType);

                        if (type is SynthesizedTypeSymbol sts)
                        {
                            var m = new SourceMethodSymbol(sts, function);

                            _extendMethods.Add(m);
                            sts.AddMember(m);
                        }
                    }
                }
            }

            var views = _sourceCode.GetViews();

            foreach (var view in views)
            {
                _types.Add(new SourceViewTypeSymbol(SourceTypeContainer, view));
            }

        }

        /// <summary>
        /// Gets compilation syntax trees.
        /// </summary>
        public ImmutableArray<AquilaSyntaxTree> SyntaxTrees => _sourceCode.SyntaxTrees;

        public ImmutableArray<AquilaSyntaxTree> Views => _views.ToImmutableArray();

        public IEnumerable<MethodSymbol> GetMethods()
        {
            foreach (var g in _globalMethods)
            {
                yield return g;
            }

            foreach (var em in _extendMethods)
            {
                yield return em;
            }

            foreach (var m in _types.SelectMany(x => x.GetMembers().OfType<MethodSymbol>()))
            {
                yield return m;
            }

            foreach (var m in _types.SelectMany(x =>
                         x.GetTypeMembers().SelectMany(y => y.GetMembers().OfType<MethodSymbol>())))
            {
                yield return m;
            }
        }

        /// <summary>
        /// Gets enumeration of all methods (global code, functions, lambdas and class methods) in source code.
        /// </summary>
        public IEnumerable<SourceMethodSymbolBase> GetSourceMethods() => GetMethods().OfType<SourceMethodSymbolBase>();

        public IEnumerable<SourceModuleTypeSymbol> GetModuleTypes() => _types.OfType<SourceModuleTypeSymbol>();

        public IEnumerable<SourceViewTypeSymbol> GetViewTypes() => _types.OfType<SourceViewTypeSymbol>();

        public SourceModuleTypeSymbol GetModuleType(string name) => GetModuleTypes().FirstOrDefault(x => x.Name == name);

        public MergedSourceCode GetMergedSourceCode() => _sourceCode;

        public NamedTypeSymbol GetType(QualifiedName name, Dictionary<QualifiedName, INamedTypeSymbol> resolved = null)
        {
            var resolvedTypes = _types.Where(x => x.MakeQualifiedName() == name).ToImmutableArray();

            if (resolvedTypes.Length == 1)
            {
                return resolvedTypes.First();
            }
            else if (resolvedTypes.Length > 1)
            {
                return new AmbiguousErrorTypeSymbol(resolvedTypes);
            }

            return new MissingMetadataTypeSymbol(name.ClrName(), 0, false);
        }


        public void AddSyntaxTreeRange(IEnumerable<AquilaSyntaxTree> syntaxTrees)
        {
            _sourceCode.AddSyntaxTreeRange(syntaxTrees);
            UpdateSymbolsCore();
        }

        public int FilesCount => _sourceCode.SyntaxTrees.Length;
    }
    
}