using ZenPlatfrom.Language.AST.Infrastructure;

namespace ZenPlatfrom.Language.AST.Definitions.Expression
{
    /// <summary>
    /// Binary expression
    /// </summary>
    public class BinaryExpression : Infrastructure.Expression
    {
        public Infrastructure.Expression Right = null;
        public Infrastructure.Expression Left = null;
        public BinaryOperatorType BinaryOperatorType;

        public BinaryExpression(Infrastructure.Expression right, Infrastructure.Expression left, BinaryOperatorType binaryOperatorType)
        {
            Right = right;
            Left = left;
            BinaryOperatorType = binaryOperatorType;
        }
    }
}