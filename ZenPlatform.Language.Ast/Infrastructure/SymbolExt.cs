using System.Net.NetworkInformation;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Statements;

namespace ZenPlatform.Language.Ast.Infrastructure
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