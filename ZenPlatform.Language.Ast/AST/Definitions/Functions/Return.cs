using System.Net.WebSockets;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Definitions.Statements;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions.Functions
{
    /// <summary>
    /// Describes a return statement.
    /// </summary>
    public class Return : Statement
    {
        private const int VALUE_SLOT = 0;

        private Expression _value;

        /// <summary>
        /// Return value;
        /// </summary>
        public Expression Value => _value;

        /// <summary>
        /// Create return object.
        /// </summary>
        /// <param name="value"></param>
        public Return(ILineInfo li, Expression value) : base(li)
        {
            _value = Children.SetSlot(value, VALUE_SLOT);
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitReturn(this);
        }
    }
}