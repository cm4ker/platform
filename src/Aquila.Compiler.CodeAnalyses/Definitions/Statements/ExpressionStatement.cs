namespace Aquila.Language.Ast.Definitions.Statements
{
    public partial class ExpressionStatement : Ast.Statement
    {
        public ExpressionStatement(Ast.Expression exp) : this(exp, exp)
        {
        }
    }
}