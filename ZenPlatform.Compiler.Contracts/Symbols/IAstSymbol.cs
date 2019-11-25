namespace ZenPlatform.Compiler.Contracts.Symbols
{
    public interface IAstSymbol
    {
        string Name { get; }

        SymbolType SymbolType { get; }

        SymbolScopeBySecurity SymbolScope { get; set; }
    }
}