using Aquila.Language.Ast.Infrastructure;
using Aquila.Language.Ast.Misc;

namespace Aquila.Language.Ast.Definitions
{
    /// <summary>
    /// Describes an array or structure initialization element.
    /// </summary>
    public partial class Element : SyntaxNode
    {
        public ElementType ElementType;
        public ElementCollection Elements;
        public Ast.Expression Expression;

        /// <summary>
        /// Creates an element collection element object.
        /// </summary>
        public Element(ILineInfo li, ElementCollection elements) : base(li)
        {
            ElementType = ElementType.Collection;
            Elements = elements;
        }

        /// <summary>
        /// Creates a expression element object.
        /// </summary>
        public Element(Ast.Expression expression) : base(expression)
        {
            ElementType = ElementType.Expression;
            Expression = expression;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new System.NotImplementedException();
        }

        public override void Accept(AstVisitorBase visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}