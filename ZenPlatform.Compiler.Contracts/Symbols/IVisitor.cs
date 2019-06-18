namespace ZenPlatform.Compiler.Contracts.Symbols
{
    public interface IVisitor
    {
        void Visit(IVisitable visitable);
    }
}