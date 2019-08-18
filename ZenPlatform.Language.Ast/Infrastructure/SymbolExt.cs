using System.Net.NetworkInformation;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Statements;

namespace ZenPlatform.Language.Ast.Infrastructure
{
    public static class SymbolExt
    {
        public static SymbolScope GetScope(this Expression exp)
        {
            return exp.FirstParent<IAstSymbol>().SymbolScope;
        }

        public static SymbolScope GetScope(this Statement exp)
        {
            return exp.FirstParent<IAstSymbol>().SymbolScope;
        }
    }
}