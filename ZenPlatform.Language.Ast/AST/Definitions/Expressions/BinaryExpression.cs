using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions.Expressions
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

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(Right);
            visitor.Visit(Left);
        }

        public override TypeNode Type
        {
            get => Left.Type;
        }
    }
}