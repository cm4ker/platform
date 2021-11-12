using System.Collections.Generic;
using Aquila.CodeAnalysis;
using Aquila.Syntax.Text;

namespace Aquila.Syntax
{
    public record SyntaxToken : LangElement
    {
        internal SyntaxToken(Span span, SyntaxKind kind, string text)
            : base(span, kind)
        {
            Text = text;
        }

        public string Text { get; }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield break;
        }
        
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