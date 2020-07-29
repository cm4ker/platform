using System.Collections.Generic;
using System.Collections.Immutable;

namespace Aquila.Language.Ast.Symbols
{
    public class SZArray : ArrayTypeSymbol
    {
        public override IEnumerable<Symbol> GetMembers(string name)
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerable<NamedTypeSymbol> GetTypeMembers(string name)
        {
            throw new System.NotImplementedException();
        }
    }
}