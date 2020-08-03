using Aquila.Syntax.Text;

namespace Aquila.Syntax
{
    public class SyntaxToken : SyntaxNode
    {
        internal SyntaxToken(Span span, SyntaxKind kind, string text)
            : base(span, kind)
        {
            Text = text;
        }

        public string Text { get; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitSyntaxToken(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitSyntaxToken(this);
        }
    }
}