namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class FieldExpression : Expression
    {
        public FieldExpression(Expression exp, string fieldName) : this(null, exp, fieldName)
        {
        }
    }
}