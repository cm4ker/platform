using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using Microsoft.CodeAnalysis;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Symbols.PE
{
    internal sealed class PEGlobalNamespaceSymbol : PENamespaceSymbol
    {
        /// <summary>
        /// The module containing the namespace.
        /// </summary>
        /// <remarks></remarks>
        readonly PEModuleSymbol _moduleSymbol;

        internal PEGlobalNamespaceSymbol(PEModuleSymbol moduleSymbol)
        {
            Debug.Assert((object) moduleSymbol != null);
            _moduleSymbol = moduleSymbol;
        }

        public override Symbol ContainingSymbol => _moduleSymbol;

        internal override PEModuleSymbol ContainingPEModule => _moduleSymbol;

        public override string Name => string.Empty;

        public override bool IsGlobalNamespace => true;

        public override AssemblySymbol ContainingAssembly => _moduleSymbol.ContainingAssembly;

        internal override ModuleSymbol ContainingModule => _moduleSymbol;

        protected override void EnsureAllMembersLoaded()
        {
            if (lazyTypes == null || lazyNamespaces == null)
            {
                IEnumerable<IGrouping<string, TypeDefinitionHandle>> groups;
 
                try
                {
                    groups = _moduleSymbol.Module.GroupTypesByNamespaceOrThrow(System.StringComparer.Ordinal);
                }
                catch (BadImageFormatException)
                {
                    groups = SpecializedCollections.EmptyEnumerable<IGrouping<string, TypeDefinitionHandle>>();
                }
 
                LoadAllMembers(groups);
            }
        }

        internal override AquilaCompilation DeclaringCompilation => null;
    }
}