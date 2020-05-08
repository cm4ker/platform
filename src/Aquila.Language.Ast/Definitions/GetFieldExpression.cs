namespace Aquila.Language.Ast.Definitions
{
    public partial class GetFieldExpression : Expression
    {
        public GetFieldExpression(Expression exp, string fieldName) : this(null, exp, fieldName)
        {
        }
    }
}