using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Aquila.Compiler.Roslyn;
using Aquila.Language.Ast.Extension;

namespace Aquila.Language.Ast.Symbols
{
    public abstract class NamespaceOrTypeSymbol : Symbol
    {
        public abstract IEnumerable<Symbol> GetMembers();

        /// <summary>
        /// Get all the members of this symbol that have a particular name.
        /// </summary>
        /// <returns>An ImmutableArray containing all the members of this symbol with the given name. If there are
        /// no members with this name, returns an empty ImmutableArray. Never returns null.</returns>
        public abstract IEnumerable<Symbol> GetMembers(string name);


        /// <summary>
        /// Lookup an immediately nested type referenced from metadata, names should be
        /// compared case-sensitively.
        /// </summary>
        /// <param name="emittedTypeName">
        /// Simple type name, possibly with generic name mangling.
        /// </param>
        /// <returns>
        /// Symbol for the type, or MissingMetadataSymbol if the type isn't found.
        /// </returns>
        internal virtual NamedTypeSymbol LookupMetadataType(ref MetadataTypeName emittedTypeName)
        {
            Debug.Assert(!emittedTypeName.IsNull);

            NamespaceOrTypeSymbol scope = this;

            if (scope.Kind == SymbolKind.ErrorType)
            {
                throw new Exception("Missing");
            }

            NamedTypeSymbol? namedType = null;

            ImmutableArray<NamedTypeSymbol> namespaceOrTypeMembers;
            bool isTopLevel = false; //scope.IsNamespace;

            if (emittedTypeName.IsMangled)
            {
                Debug.Assert(!emittedTypeName.UnmangledTypeName.Equals(emittedTypeName.TypeName) &&
                             emittedTypeName.InferredArity > 0);

                if (emittedTypeName.ForcedArity == -1 || emittedTypeName.ForcedArity == emittedTypeName.InferredArity)
                {
                    // Let's handle mangling case first.
                    namespaceOrTypeMembers = scope.GetTypeMembers(emittedTypeName.UnmangledTypeName).ToImmutableArray();

                    // foreach (var named in namespaceOrTypeMembers)
                    // {
                    //     if (emittedTypeName.InferredArity == named.Arity && named.MangleName)
                    //     {
                    //         if ((object?) namedType != null)
                    //         {
                    //             namedType = null;
                    //             break;
                    //         }
                    //
                    //         namedType = named;
                    //     }
                    // }
                }
            }
            else
            {
                Debug.Assert(ReferenceEquals(emittedTypeName.UnmangledTypeName, emittedTypeName.TypeName) &&
                             emittedTypeName.InferredArity == 0);
            }

            // Now try lookup without removing generic arity mangling.
            int forcedArity = emittedTypeName.ForcedArity;

            if (emittedTypeName.UseCLSCompliantNameArityEncoding)
            {
                // Only types with arity 0 are acceptable, we already examined types with mangled names.
                if (emittedTypeName.InferredArity > 0)
                {
                    goto Done;
                }
                else if (forcedArity == -1)
                {
                    forcedArity = 0;
                }
                else if (forcedArity != 0)
                {
                    goto Done;
                }
                else
                {
                    Debug.Assert(forcedArity == emittedTypeName.InferredArity);
                }
            }

            namespaceOrTypeMembers = scope.GetTypeMembers(emittedTypeName.TypeName).ToImmutableArray();

            foreach (var named in namespaceOrTypeMembers)
            {
                if ((object?) namedType != null)
                {
                    namedType = null;
                    break;
                }

                namedType = named;
            }

            Done:
            if ((object?) namedType == null)
            {
                if (isTopLevel)
                {
                }
                else
                {
                }
            }

            return namedType;
        }


        /// <summary>
        /// Get all the members of this symbol that are types that have a particular name, of any arity.
        /// </summary>
        /// <returns>An ImmutableArray containing all the types that are members of this symbol with the given name.
        /// If this symbol has no type members with this name,
        /// returns an empty ImmutableArray. Never returns null.</returns>
        public abstract IEnumerable<NamedTypeSymbol> GetTypeMembers(string name);
    }
}