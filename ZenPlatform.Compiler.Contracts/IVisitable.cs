namespace ZenPlatform.Compiler.Visitor
{
    public interface IVisitor
    {
        void Visit(IVisitable visitable);
    }

    public interface IVisitable
    {
        void Accept(IVisitor visitor);
    }
}