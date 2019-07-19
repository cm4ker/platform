using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Compiler.Contracts.Symbols
{
    public interface IAstNode : ILineInfo, IVisitable
    {
        int Line { get; set; }
        int Position { get; set; }
    }
}