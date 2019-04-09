namespace PrototypePlatformLanguage.AST
{
    public class BinaryExpression : Expression
    {
        public Expression Right = null;
        public Expression Left = null;
        public BinaryOperatorType BinaryOperatorType = BinaryOperatorType.None;

        public BinaryExpression(Expression right, Expression left, BinaryOperatorType binaryOperatorType)
        {
            Right = right;
            Left = left;
            BinaryOperatorType = binaryOperatorType;
        }
    }
}