namespace ZenPlatform.Language.Ast.Definitions.Statements
{
    public partial class ExpressionStatement : Statement
    {
        public ExpressionStatement(Expression exp) : this(exp, exp)
        {
        }
    }
}