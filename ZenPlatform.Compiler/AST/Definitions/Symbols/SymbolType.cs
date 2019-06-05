namespace ZenPlatform.Compiler.AST.Definitions.Symbols
{
    public enum SymbolType
    {
        None,
        Function,
        Type,
        Variable
    }


    public interface IAstSymbol
    {
        string Name { get; set; }
        SymbolType SymbolType { get; }
    }
}