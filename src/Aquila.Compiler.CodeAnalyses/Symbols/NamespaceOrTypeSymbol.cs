using System.Collections.Generic;

namespace Aquila.Language.Ast.Symbols
{
    public abstract class NamespaceOrTypeSymbol : Symbol
    {
        public abstract IEnumerable<Symbol> GetMembers();
    }
}