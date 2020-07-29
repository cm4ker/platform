using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Aquila.Language.Ast.Symbols.PE
{
    public sealed class PEGlobalNamespaceSymbol : PENamespaceSymbol
    {
        private readonly PEModuleSymbol _moduleSymbol;
        private bool _loaded;

        public PEGlobalNamespaceSymbol(PEModuleSymbol moduleSymbol)
        {
            _moduleSymbol = moduleSymbol;
        }

        public override IEnumerable<Symbol> GetMembers()
        {
            return null;
        }

        internal void LoadAllTypes()
        {
            if (_loaded) return;
            
            _loaded = true;
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
                var key = enumerator.Current.Key;
                if (enumerator.Current.Key == null)
                {
                    enumerator.MoveNext();
                    continue;
                }

                var currentNamespace = enumerator.Current;
                var nested = new PENestedNamespaceSymbol(currentNamespace.Key, this);
                Nested.Add(currentNamespace.Key, nested);
                nested.LoadNested(enumerator);


                //strange but Enumerator not unckeck the Current status
                if (key == enumerator.Current.Key)
                    break;
            }
        }

        internal override PEModuleSymbol ContainingPEModule => _moduleSymbol;
    }
}