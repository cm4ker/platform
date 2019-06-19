namespace ZenPlatform.Compiler.Contracts.Symbols
{
    public interface IVisitable
    {
        void Accept<T>(IVisitor<T> visitor);
    }
}