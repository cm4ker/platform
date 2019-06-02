namespace ZenPlatform.Compiler.AST.Definitions.Symbols
{
    public enum SymbolType
    {
        None,
        Function,
        Structure,
        Variable
    }


    public interface IAstSymbol
    {
        string Name { get; set; }
        SymbolType SymbolType { get; }
    }
}