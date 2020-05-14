using Aquila.Shared.ParenChildCollection;

namespace Aquila.Compiler.Contracts.Symbols
{
    public interface IAstNode : ILineInfo
    {
        int Line { get; set; }
        int Position { get; set; }
    }
}