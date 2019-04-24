using ZenPlatform.Language.AST.Definitions.Statements;

namespace ZenPlatform.Language.AST.Definitions.Functions
{
    /// <summary>
    /// Describes a return statement.
    /// </summary>
    public class Return : Statement
    {
        /// <summary>
        /// Return value;
        /// </summary>
        public Infrastructure.Expression Value;

        /// <summary>
        /// Create return object.
        /// </summary>
        /// <param name="value"></param>
        public Return(Infrastructure.Expression value)
        {
            Value = value;
        }
    }
}