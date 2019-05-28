namespace ZenPlatform.Compiler.Visitor
{
    public interface IVisitable
    {
        T Accept<T>(IVisitor visitor);
    }

    public interface IVisitor
    
    {
        T Visit<T>(IVisitable visitable);
    }
}