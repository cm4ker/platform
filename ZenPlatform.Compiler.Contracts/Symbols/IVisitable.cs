namespace ZenPlatform.Compiler.Contracts.Symbols
{
    public interface IVisitable
    {
        void Accept(IVisitor visitor);
    }
}