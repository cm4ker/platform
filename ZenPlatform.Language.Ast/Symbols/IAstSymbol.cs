using ZenPlatform.Compiler.Contracts.Symbols;

namespace ZenPlatform.Language.Ast.Symbols
{
    public interface IAstSymbol
    {
        string Name { get; }

        SymbolType SymbolType { get; }

        SymbolScopeBySecurity SymbolScope { get; set; }
    }
}