namespace ZenPlatform.Compiler.Contracts.Symbols
{
    public interface ILineInfo
    {
        int Line { get; set; }
        int Position { get; set; }
    }
}