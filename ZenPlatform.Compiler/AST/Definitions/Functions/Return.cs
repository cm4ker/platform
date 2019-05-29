using ZenPlatform.Compiler.AST.Definitions.Statements;
using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Visitor;

namespace ZenPlatform.Compiler.AST.Definitions.Functions
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
        public Return(ILineInfo li, Expression value) : base(li)
        {
            Value = value;
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(Value);
        }
    }
}