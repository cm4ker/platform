namespace PrototypePlatformLanguage.AST
{
    /// <summary>
    /// Describes an argument.
    /// </summary>
    public class Argument
    {
        /// <summary>
        /// Argument expression.
        /// </summary>
        public Expression Value;

        /// <summary>
        /// Argument pass method.
        /// </summary>
        public PassMethod PassMethod = PassMethod.ByValue;

        /// <summary>
        /// Create argument object.
        /// </summary>
        public Argument(Expression value, PassMethod passMethod)
        {
            Value = value;
            PassMethod = passMethod;
        }
    }
}