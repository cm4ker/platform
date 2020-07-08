using Aquila.Language.Ast.Symbols;

namespace Aquila.Language.Ast.Binding
{
    internal sealed class BoundBinaryExpression : BoundExpression
    {
        public BoundBinaryExpression(SyntaxNode syntax, BoundExpression left, BoundBinaryOperator op, BoundExpression right)
            : base(syntax)
        {
            Left = left;
            Op = op;
            Right = right;
            ConstantValue = ConstantFolding.Fold(left, op, right);
        }

        public override BoundNodeKind Kind => BoundNodeKind.BinaryExpression;
        public override NamedTypeSymbol NamedType => Op.NamedType;
        public BoundExpression Left { get; }
        public BoundBinaryOperator Op { get; }
        public BoundExpression Right { get; }
        public override BoundConstant? ConstantValue { get; }
    }
}
