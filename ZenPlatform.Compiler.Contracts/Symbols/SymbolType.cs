namespace ZenPlatform.Compiler.Contracts.Symbols
{
    public enum SymbolType
    {
        None,
        Function,
        Type,
        Variable,
        Field
    }


    public interface IAstSymbol
    {
        string Name { get; set; }
        SymbolType SymbolType { get; }
    }
}