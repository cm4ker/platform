namespace ZenPlatform.Compiler.Contracts.Symbols
{
    public interface IVisitor<T>
    {
        T Visit(IVisitable visitable);
    }
}