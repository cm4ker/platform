using Aquila.Language.Ast.Symbols;

namespace Aquila.Language.Ast.Binding
{
    internal sealed class BoundCompoundAssignmentExpression : BoundExpression
    {
        public BoundCompoundAssignmentExpression(SyntaxNode syntax, LocalSymbol local, BoundBinaryOperator op, BoundExpression expression)
            : base(syntax)
        {
            Local = local;
            Op = op;
            Expression = expression;
        }

        public override BoundNodeKind Kind => BoundNodeKind.CompoundAssignmentExpression;
        public override NamedTypeSymbol NamedType => Expression.NamedType;
        public LocalSymbol Local { get; }
        public BoundBinaryOperator Op {get; }
        public BoundExpression Expression { get; }
    }
}
