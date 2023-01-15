using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Aquila.CodeAnalysis.Symbols.Attributes;
using Aquila.CodeAnalysis.Symbols.Source;
using Microsoft.CodeAnalysis;
using ImmutableArrayExtensions = System.Linq.ImmutableArrayExtensions;

namespace Aquila.CodeAnalysis.Symbols
{
    internal sealed class SourceModuleSymbol : ModuleSymbol, IModuleSymbol
    {
        readonly SourceAssemblySymbol _sourceAssembly;
        readonly string _name;
        readonly NamespaceSymbol _ns;

        /// <summary>
        /// Tables of all source symbols to be compiled within the source module.
        /// </summary>
        public SourceSymbolCollection SymbolCollection => DeclaringCompilation.SourceSymbolCollection;

        public SourceModuleSymbol(SourceAssemblySymbol sourceAssembly, string name)
        {
            _sourceAssembly = sourceAssembly;
            _name = name;
            _ns = new SourceGlobalNamespaceSymbol(this);
        }

        public override string Name => _name;

        public override Symbol ContainingSymbol => _sourceAssembly;

        public override NamespaceSymbol GlobalNamespace => _ns;

        internal SourceAssemblySymbol SourceAssemblySymbol => _sourceAssembly;

        public override AssemblySymbol ContainingAssembly => _sourceAssembly;

        public override ImmutableArray<Location> Locations
        {
            get { throw new NotImplementedException(); }
        }

        internal override AquilaCompilation DeclaringCompilation => _sourceAssembly.DeclaringCompilation;

        /// <summary>
        /// Lookup a top level type referenced from metadata, names should be
        /// compared case-sensitively.
        /// </summary>
        /// <param name="emittedName">
        /// Full type name, possibly with generic name mangling.
        /// </param>
        /// <returns>
        /// Symbol for the type, or MissingMetadataSymbol if the type isn't found.
        /// </returns>
        /// <remarks></remarks>
        internal sealed override NamedTypeSymbol LookupTopLevelMetadataType(ref MetadataTypeName emittedName)
        {
            NamedTypeSymbol result;
            NamespaceSymbol scope = this.GlobalNamespace; 

            if ((object)scope == null)
            {
                // We failed to locate the namespace
                throw new NotImplementedException();
            }

            result = scope.LookupMetadataType(ref emittedName);
            Debug.Assert((object)result != null);
            return result;
        }

        internal override ICollection<string> TypeNames => ArraySegment<string>.Empty;
        internal override ICollection<string> NamespaceNames => ArraySegment<string>.Empty;

        ImmutableArray<AttributeData> _lazyAttributesToEmit;

        internal override IEnumerable<AttributeData> GetCustomAttributesToEmit(
            CommonModuleCompilationState compilationState)
        {
            var attrs = base.GetCustomAttributesToEmit(compilationState);

            if (_lazyAttributesToEmit.IsDefault)
            {
                _lazyAttributesToEmit = CreateAttributesToEmit().ToImmutableArray();
            }

            attrs = Enumerable.Concat(attrs, _lazyAttributesToEmit);

            //
            return attrs;
        }

        IEnumerable<AttributeData> CreateAttributesToEmit()
        {
            yield break;
        }
    }
}