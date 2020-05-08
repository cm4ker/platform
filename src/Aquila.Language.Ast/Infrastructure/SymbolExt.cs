using System.Net.NetworkInformation;
using Aquila.Compiler.Contracts.Symbols;
using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Definitions.Statements;
using Aquila.Language.Ast.Symbols;

namespace Aquila.Language.Ast.Infrastructure
{
    public static class SymbolExt
    {
        public static SymbolScopeBySecurity GetScope(this Expression exp)
        {
            return exp.FirstParent<IAstSymbol>().SymbolScope;
        }

        public static SymbolScopeBySecurity GetScope(this Statement exp)
        {
            return exp.FirstParent<IAstSymbol>().SymbolScope;
        }
    }
}