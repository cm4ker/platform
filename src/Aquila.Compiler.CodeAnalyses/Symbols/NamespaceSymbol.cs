using System.Collections.Immutable;
using System.Diagnostics;

namespace Aquila.Language.Ast.Symbols
{
    public abstract class NamespaceSymbol : NamespaceOrTypeSymbol
    {
        public override SymbolKind Kind => SymbolKind.Namespace;
        
        /// <summary>
        /// Lookup a nested namespace.
        /// </summary>
        /// <param name="names">
        /// Sequence of names for nested child namespaces.
        /// </param>
        /// <returns>
        /// Symbol for the most nested namespace, if found. Nothing 
        /// if namespace or any part of it can not be found.
        /// </returns>
        internal NamespaceSymbol LookupNestedNamespace(ImmutableArray<string> names)
        {
            NamespaceSymbol scope = this;

            foreach (string name in names)
            {
                NamespaceSymbol nextScope = null;

                foreach (NamespaceOrTypeSymbol symbol in scope.GetMembers(name))
                {
                    var ns = symbol as NamespaceSymbol;

                    if ((object)ns != null)
                    {
                        if ((object)nextScope != null)
                        {
                            Debug.Assert((object)nextScope == null, "Why did we run into an unmerged namespace?");
                            nextScope = null;
                            break;
                        }

                        nextScope = ns;
                    }
                }

                scope = nextScope;

                if ((object)scope == null)
                {
                    break;
                }
            }

            return scope;
        }
    }
}