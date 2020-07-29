using System.Collections.Generic;
using System.Collections.Immutable;

namespace Aquila.Language.Ast.Symbols
{
    public abstract class NamedTypeSymbol : TypeSymbol
    {
        public override SymbolKind Kind => SymbolKind.NamedType;

        public override IEnumerable<Symbol> GetMembers() => null;

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