using System.Collections.Generic;

namespace Aquila.Language.Ast.Symbols
{
    public class NamedTypeSymbol : TypeSymbol
    {
        public override SymbolKind Kind => SymbolKind.NamedType;

        public override IEnumerable<Symbol> GetMembers() => null;
    }
}