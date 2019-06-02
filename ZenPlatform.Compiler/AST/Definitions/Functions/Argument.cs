using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Visitor;

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
        public Expression Value;

        /// <summary>
        /// Argument pass method.
        /// </summary>
        public PassMethod PassMethod = PassMethod.ByValue;

        /// <summary>
        /// Create argument object.
        /// </summary>
        public Argument(ILineInfo lineInfo, Expression value, PassMethod passMethod) : base(lineInfo)
        {
            Value = value;
            PassMethod = passMethod;
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(Value);
        }
    }
}