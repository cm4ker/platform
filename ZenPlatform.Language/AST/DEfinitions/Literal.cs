namespace ZenPlatfrom.Language.AST.Definitions
{
    /// <summary>
    /// Describes a literal found in text
    /// </summary>
    public class Literal : Infrastructure.Expression
    {
        /// <summary>
        /// Textual value of the literal, string and character literals include quotes.
        /// </summary>
        public string Value;

        /// <summary>
        /// Type of literal.
        /// </summary>
        public LiteralType LiteralType;

        /// <summary>
        /// Create a literal object.
        /// </summary>
        /// <param name="value">Value of the literal.</param>
        /// <param name="literalType">Literal type.</param>
        public Literal(string value, LiteralType literalType)
        {
            Value = value;
            LiteralType = literalType;
        }
    }
}