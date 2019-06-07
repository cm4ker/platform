using ZenPlatform.Compiler.Visitor;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions
{
    /// <summary>
    /// Describes an array or structure initialization element.
    /// </summary>
    public class Element : AstNode
    {
        public ElementType ElementType;
        public ElementCollection Elements;
        public Expression Expression;

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
        public Element(Expression expression) : base(expression)
        {
            ElementType = ElementType.Expression;
            Expression = expression;
        }

        public override void Accept(IVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}