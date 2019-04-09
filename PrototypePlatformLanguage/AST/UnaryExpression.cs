namespace PrototypePlatformLanguage.AST
{
    public class UnaryExpression : Expression
    {
        public Expression Value = null;
        public Expression Indexer = null;
        public UnaryOperatorType UnaryOperatorType = UnaryOperatorType.None;

        public UnaryExpression(Expression indexer, Expression value, UnaryOperatorType unaryOperatorType)
        {
            Value = value;
            Indexer = indexer;
            UnaryOperatorType = unaryOperatorType;
        }
    }
}