namespace PrototypePlatformLanguage.AST
{
    /// <summary>
    /// Describes a return statement.
    /// </summary>
    public class Return : Statement
    {
        /// <summary>
        /// Return value;
        /// </summary>
        public Expression Value;

        /// <summary>
        /// Create return object.
        /// </summary>
        /// <param name="value"></param>
        public Return(Expression value)
        {
            Value = value;
        }
    }
}