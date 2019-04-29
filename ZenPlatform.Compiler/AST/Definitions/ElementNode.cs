using ZenPlatform.Compiler.AST.Infrastructure;

namespace ZenPlatform.Compiler.AST.Definitions
{
    /// <summary>
    /// Describes an array or structure initialization element.
    /// </summary>
    public class ElementNode : AstNode
    {
        public ElementType ElementType = ElementType.Expression;
        public ElementCollection Elements = null;
        public Infrastructure.Expression Expression = null;

        /// <summary>
        /// Creates an element collection element object.
        /// </summary>
        public ElementNode(ElementCollection elements)
        {
            ElementType = ElementType.Collection;
            Elements = elements;
        }

        /// <summary>
        /// Creates a expression element object.
        /// </summary>
        public ElementNode(Infrastructure.Expression expression)
        {
            ElementType = ElementType.Expression;
            Expression = expression;
        }
    }
}