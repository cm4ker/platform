using System.Collections.Generic;

namespace Aquila.Language.Ast.Symbols.PE
{
    public sealed class PEGlobalNamespaceSymbol : PENamespaceSymbol
    {
        private readonly PEModuleSymbol _moduleSymbol;
        private IEnumerable<NamedTypeSymbol> _types;

        public PEGlobalNamespaceSymbol(PEModuleSymbol moduleSymbol)
        {
            _moduleSymbol = moduleSymbol;

            LoadAllTypes();
        }

        public override IEnumerable<Symbol> GetMembers()
        {
            return _types;
        }


        private void LoadAllTypes()
        {
            var types = _moduleSymbol.Module.GetTypes().ToList();
            var groupedTypes =
                types.GroupBy(x => x.Namespace)
                    .OrderBy(x => x.Key);
            /*
             System;  <-------
                             |
                nested for ---
             System.Collections;  <------
                                        |
                          nested for ---
             System.Collections.Generic;
             */
            using var enumerator = groupedTypes.GetEnumerator();
            enumerator.MoveNext();

            while (enumerator.Current != null)
            {
                var currentNamespace = enumerator.Current;
                var nested = new PENestedNamespaceSymbol(currentNamespace.Key, this);
                _nested.Add(nested);
                nested.LoadNested(enumerator);
            }
        }

        internal override PEModuleSymbol ContainingPEModule => _moduleSymbol;
    }
}