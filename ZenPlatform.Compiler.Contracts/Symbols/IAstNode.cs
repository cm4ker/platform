using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Compiler.Contracts.Symbols
{
    public interface IAstNode : ILineInfo, IChildItem<IAstNode>, IVisitable
    {
        int Line { get; set; }
        int Position { get; set; }
        IAstNode Parent { get; set; }
        T GetParent<T>() where T : IAstNode;
        void Accept(IVisitor visitor);
    }
}