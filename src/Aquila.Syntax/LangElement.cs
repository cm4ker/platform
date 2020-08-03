using Aquila.Shared.Tree;
using Aquila.Syntax.Text;

namespace Aquila.Syntax
{
    public abstract class LangElement : Node
    {
        protected LangElement(Span span, SyntaxKind kind)
        {
            Kind = kind;
            Span = span;
        }

        public SyntaxKind Kind { get; }

        public Span Span { get; }

        public abstract T Accept<T>(AstVisitorBase<T> visitor);

        public abstract void Accept(AstVisitorBase visitor);
    }
}