using Aquila.Compiler.Contracts.Symbols;

namespace Aquila.Language.Ast.Symbols
{
    public interface IAstSymbol
    {
        string Name { get; }

        SymbolType SymbolType { get; }

        SymbolScopeBySecurity SymbolScope { get; set; }
    }
}