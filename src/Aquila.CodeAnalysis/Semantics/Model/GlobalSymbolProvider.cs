using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.Symbols;
using System.Collections.Immutable;
using System.Diagnostics;
using Aquila.CodeAnalysis.Symbols.Aquila;
using Aquila.CodeAnalysis.Symbols.PE;
using Aquila.CodeAnalysis.Utilities;
using Aquila.Compiler.Utilities;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Semantics.Model
{
    internal class GlobalSymbolProvider : ISymbolProvider
    {
        #region Fields

        readonly AquilaCompilation _compilation;
        readonly ISymbolProvider _next;

        ImmutableArray<NamedTypeSymbol> _lazyExtensionContainers;

        /// <summary>
        /// Types that are visible from extension libraries.
        /// </summary>
        Dictionary<QualifiedName, NamedTypeSymbol> _lazyExportedTypes;

        /// <summary>
        /// Functions that are visible from extension libraries.
        /// </summary>
        MultiDictionary<QualifiedName, MethodSymbol> _lazyExportedFunctions;

        #endregion

        public GlobalSymbolProvider(AquilaCompilation compilation)
        {
            _compilation = compilation ?? throw new ArgumentNullException(nameof(compilation));
            _next = new SourceSymbolProvider(compilation.SourceSymbolCollection);
        }


        internal bool IsFunction(MethodSymbol method)
        {
            return method != null && method.IsStatic && method.DeclaredAccessibility == Accessibility.Public &&
                   method.MethodKind == MethodKind.Ordinary && !method.IsAquilaHidden(_compilation);
        }

        internal bool IsGlobalConstant(Aquila.CodeAnalysis.Symbols.Symbol symbol)
        {
            if (symbol is FieldSymbol field)
            {
                return (field.IsConst || (field.IsReadOnly && field.IsStatic)) &&
                       field.DeclaredAccessibility == Accessibility.Public &&
                       !field.IsAquilaHidden(_compilation);
            }

            if (symbol is PropertySymbol prop)
            {
                return prop.IsStatic && prop.DeclaredAccessibility == Accessibility.Public &&
                       !prop.IsAquilaHidden(_compilation);
            }

            return false;
        }

        /// <summary>
        /// (Aquila) Types exported from extension libraries and cor library.
        /// </summary>
        public Dictionary<QualifiedName, NamedTypeSymbol> ExportedTypes
        {
            get
            {
                if (_lazyExportedTypes == null)
                {
                    var result = new Dictionary<QualifiedName, NamedTypeSymbol>();

                    // lookup extensions and cor library for exported types
                    var libs = new List<PEAssemblySymbol>();
                    libs.Add((PEAssemblySymbol)_compilation.AquilaCorLibrary);

                    //
                    foreach (var lib in libs)
                    {
                        foreach (var t in lib.PrimaryModule.GlobalNamespace.GetTypeMembers()
                                     .OfType<PENamedTypeSymbol>())
                        {
                            if (t.DeclaredAccessibility == Accessibility.Public)
                            {
                                var qname = t.GetAquilaTypeNameOrNull();
                                if (!qname.IsEmpty())
                                {
                                    NamedTypeSymbol tsymbol = t;

                                    if (result.TryGetValue(qname, out var existing))
                                    {
                                        if (existing is AmbiguousErrorTypeSymbol ambiguous)
                                        {
                                            // just collect possible types, there is perf. penalty for that
                                            // TODO: if there are user & library types mixed together, we expect compilation assertions and errors, fix that
                                            // this will be fixed once we stop declare unreachable types
                                            ambiguous._candidates = ambiguous._candidates.Add(t);
                                            continue;
                                        }
                                        else
                                        {
                                            tsymbol = new AmbiguousErrorTypeSymbol(ImmutableArray.Create(existing, t));
                                        }
                                    }

                                    result[qname] = tsymbol;
                                }
                            }
                        }
                    }

                    //
                    _lazyExportedTypes = result;
                }

                return _lazyExportedTypes;
            }
        }


        /// <summary>
        /// Gets types exported from referenced extension libraries and cor library.
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<NamedTypeSymbol> GetReferencedTypes() =>
            ExportedTypes.Values.Where(t => t.IsValidType());


        #region ISemanticModel

        /// <summary>
        /// Gets declaring compilation.
        /// </summary>
        public AquilaCompilation Compilation => _compilation;

        public INamedTypeSymbol ResolveType(QualifiedName name,
            Dictionary<QualifiedName, INamedTypeSymbol> resolved = null)
        {
            Debug.Assert(!name.IsReservedClassName);
            Debug.Assert(!name.IsEmpty());

            if (resolved != null && resolved.TryGetValue(name, out var type))
            {
                return type;
            }

            return
                ExportedTypes.TryGetOrDefault(name) ??
                GetTypeFromNonExtensionAssemblies(name.ClrName()) ??
                _next.ResolveType(name, resolved);
        }

        public NamedTypeSymbol GetTypeFromNonExtensionAssemblies(string clrName)
        {
            foreach (AssemblySymbol ass in _compilation.ProbingAssemblies)
            {
                if (ass is PEAssemblySymbol peass && !peass.IsAquilaCorLibrary)
                {
                    var candidate = ass.GetTypeByMetadataName(clrName);
                    if (candidate.IsValidType())
                    {
                        return candidate;
                    }
                }
            }

            return null;
        }

        #endregion
    }
}