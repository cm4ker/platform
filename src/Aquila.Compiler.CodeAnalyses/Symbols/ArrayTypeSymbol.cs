using System;
using System.Collections.Generic;

namespace Aquila.Language.Ast.Symbols
{
    public abstract class ArrayTypeSymbol : TypeSymbol
    {
        public override SymbolKind Kind => SymbolKind.ArrayType;

        public override IEnumerable<Symbol> GetMembers()
        {
            throw new NotImplementedException();
        }
    }
}