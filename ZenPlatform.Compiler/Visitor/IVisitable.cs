namespace ZenPlatform.Compiler.Visitor
{
    public class Visitor
    {
        private readonly VisitorContext _context;

        public Visitor(VisitorContext context)
        {
            _context = context;
        }

        public void Visit(Item item)
        {
            _context.SetVisitor(new Visitor(_context));
        }
    }

    public class VisitorContext
    {
        private Visitor _visitor;

        public void SetVisitor(Visitor visitor)
        {
            _visitor = visitor;
        }

        public void Visit(Item item)
        {
            _visitor.Visit(item);
        }
    }

    public class Item
    {
        public void Accept(VisitorContext visitor)
        {
            visitor.Visit();
        }
    }
}