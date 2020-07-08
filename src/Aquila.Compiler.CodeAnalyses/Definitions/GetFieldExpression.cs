namespace Aquila.Language.Ast.Definitions
{
    public partial class GetFieldExpression : Ast.Expression
    {
        public GetFieldExpression(Ast.Expression exp, string fieldName) : this(null, exp, fieldName)
        {
        }
    }
}