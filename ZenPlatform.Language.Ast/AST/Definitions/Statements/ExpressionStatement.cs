using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions.Statements
{
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
            return visitor.VisitExpressionStatement(this);
        }
    }
}