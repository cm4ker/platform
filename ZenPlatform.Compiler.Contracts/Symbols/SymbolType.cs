namespace ZenPlatform.Compiler.Contracts.Symbols
{
    public enum SymbolType
    {
        None,
        Function,
        Type,
        Variable,
        Field,
        Property
    }


    public interface IAstSymbol
    {
        string Name { get; set; }
        SymbolType SymbolType { get; }
    }
}