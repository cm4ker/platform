using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions.Functions
{
    /// <summary>
    /// Describes an argument.
    /// </summary>
    public class Argument : AstNode
    {
        private const int VALUE_SLOT = 0;

        private readonly Expression _value;

        /// <summary>
        /// Argument expression.
        /// </summary>
        public Expression Value => _value;

        /// <summary>
        /// Argument pass method.
        /// </summary>
        public PassMethod PassMethod { get; }

        /// <summary>
        /// Create argument object.
        /// </summary>
        public Argument(ILineInfo lineInfo, Expression value, PassMethod passMethod = PassMethod.ByValue) :
            base(lineInfo)
        {
            _value = Children.SetSlot(value, VALUE_SLOT);
            PassMethod = passMethod;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitArgument(this);
        }
    }
}