using ZenPlatform.Compiler.AST.Infrastructure;

namespace ZenPlatform.Compiler.AST.Definitions.Expressions
{
    /// <summary>
    /// Binary expression
    /// </summary>
    public class BinaryExpression : Expression
    {
        public Expression Right;
        public Expression Left;
        public BinaryOperatorType BinaryOperatorType;

        public BinaryExpression(ILineInfo li, Expression right, Expression left,
            BinaryOperatorType binaryOperatorType) : base(li)
        {
            Right = right;
            Left = left;
            BinaryOperatorType = binaryOperatorType;
        }
    }
}