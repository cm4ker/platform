using ZenPlatform.Compiler.AST.Infrastructure;

namespace ZenPlatform.Compiler.AST.Definitions.Functions
{
    /// <summary>
    /// Describes an argument.
    /// </summary>
    public class Argument : AstNode
    {
        /// <summary>
        /// Argument expression.
        /// </summary>
        public Infrastructure.Expression Value;

        /// <summary>
        /// Argument pass method.
        /// </summary>
        public PassMethod PassMethod = PassMethod.ByValue;

        /// <summary>
        /// Create argument object.
        /// </summary>
        public Argument(Infrastructure.Expression value, PassMethod passMethod)
        {
            Value = value;
            PassMethod = passMethod;
        }
    }
}