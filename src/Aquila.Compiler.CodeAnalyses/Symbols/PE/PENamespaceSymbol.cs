using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Aquila.Compiler.Contracts;
using Aquila.Language.Ast.Definitions.Statements;
using SharpDX.Direct3D11;

namespace Aquila.Language.Ast.Symbols.PE
{
    public abstract class PENamespaceSymbol : NamespaceSymbol
    {
        protected Dictionary<string, PENestedNamespaceSymbol>
            Nested = new Dictionary<string, PENestedNamespaceSymbol>();

        protected Dictionary<string, List<PENamedTypeSymbol>> Types = new Dictionary<string, List<PENamedTypeSymbol>>();


        internal abstract PEModuleSymbol ContainingPEModule { get; }

        internal void LoadNested(IEnumerator<IGrouping<string, IType>> groupEnum)
        {
            //init this instance

            var typeList = groupEnum.Current.Select<IType, PENamedTypeSymbol>(x =>
            {
                if (x.HasGenericParameters) return new PENamedTypeSymbolGeneric(x);
                else return new PENamedTypeSymbolNonGeneric(ContainingPEModule, this, x);
            }).ToList();

            Types = typeList.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.ToList());


            if (!groupEnum.MoveNext()) return;
            while (groupEnum.Current.Key.StartsWith(Name))
            {
                var nested = new PENestedNamespaceSymbol(groupEnum.Current.Key, this);
                Nested.Add(groupEnum.Current.Key, nested);
                nested.LoadNested(groupEnum);
                // var nestesdTypes = nested.Types.First();
                // Types.Add(nestesdTypes.Key, nestesdTypes.Value);
                if (!groupEnum.MoveNext()) return;
            }
        }

        public sealed override IEnumerable<Symbol> GetMembers(string name)
        {
            PENestedNamespaceSymbol ns = null;
            List<PENamedTypeSymbol> t;

            if (Nested.TryGetValue(name, out ns))
            {
                if (Types.TryGetValue(name, out t))
                {
                    var res = new List<Symbol>(t);
                    res.Add(ns);
                    return res;
                }
                else
                {
                    return ImmutableArray.Create<Symbol>(ns);
                }
            }
            // else if (lazyTypes.TryGetValue(name, out t))
            // {
            //     return StaticCast<Symbol>.From(t);
            // }

            return ImmutableArray<Symbol>.Empty;
        }

        public sealed override IEnumerable<NamedTypeSymbol> GetTypeMembers(string name)
        {
            List<PENamedTypeSymbol> t;

            Types.TryGetValue(name, out t);

            return t;
        }
    }
}