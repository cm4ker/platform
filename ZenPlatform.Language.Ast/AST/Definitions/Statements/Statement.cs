using System.ComponentModel.Design.Serialization;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions.Statements
{
    /// <summary>
    /// Describes a statement.
    /// </summary>
    public abstract class Statement : AstNode
    {
        public Statement(ILineInfo lineInfo) : base(lineInfo)
        {
        }
    }

    public class ExpressionStatement : Statement
    {
        private const int EXP_SLOT = 0;

        private Expression _expression;

        public Expression Expression => _expression;

        public ExpressionStatement(Expression expression) : base(expression)
        {
            _expression = Children.SetSlot(expression, EXP_SLOT);
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}