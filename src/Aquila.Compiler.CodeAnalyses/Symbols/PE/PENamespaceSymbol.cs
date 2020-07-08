using System.Collections.Generic;
using System.Linq;
using Aquila.Compiler.Contracts;

namespace Aquila.Language.Ast.Symbols.PE
{
    public abstract class PENamespaceSymbol : NamespaceSymbol
    {
        protected List<PENestedNamespaceSymbol> _nested = new List<PENestedNamespaceSymbol>();
        protected List<PENamedTypeSymbol> _types = new List<PENamedTypeSymbol>();

        internal abstract PEModuleSymbol ContainingPEModule { get; }

        internal void LoadNested(IEnumerator<IGrouping<string, IType>> groupEnum)
        {
            //init this instance
            _types = groupEnum.Current.Select<IType, PENamedTypeSymbol>(x =>
            {
                if (x.HasGenericParameters) return new PENamedTypeSymbolGeneric();
                else return new PENamedTypeSymbolNonGeneric(ContainingPEModule, this, x);
            }).ToList();

            if (!groupEnum.MoveNext()) return;
            if (groupEnum.Current.Key.StartsWith((string) this.Name))
            {
                var nested = new PENestedNamespaceSymbol(groupEnum.Current.Key, this);
                _nested.Add(nested);
                nested.LoadNested(groupEnum);
            }
        }
    }
}