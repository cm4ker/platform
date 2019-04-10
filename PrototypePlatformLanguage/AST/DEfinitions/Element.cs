using PrototypePlatformLanguage.AST.Infrastructure;

namespace PrototypePlatformLanguage.AST.Definitions
{
    /// <summary>
    /// Describes an array or structure initialization element.
    /// </summary>
    public class Element
    {
        public ElementType ElementType = ElementType.Expression;
        public ElementCollection Elements = null;
        public Infrastructure.Expression Expression = null;

        /// <summary>
        /// Creates an element collection element object.
        /// </summary>
        public Element(ElementCollection elements)
        {
            ElementType = ElementType.Collection;
            Elements = elements;
        }

        /// <summary>
        /// Creates a expression element object.
        /// </summary>
        public Element(Infrastructure.Expression expression)
        {
            ElementType = ElementType.Expression;
            Expression = expression;
        }
    }
}