using Aquila.Language.Ast.Symbols;

namespace Aquila.Language.Ast.Binding
{
    internal sealed class BoundAssignmentExpression : BoundExpression
    {
        public BoundAssignmentExpression(SyntaxNode syntax, LocalSymbol local, BoundExpression expression)
            : base(syntax)
        {
            Local = local;
            Expression = expression;
        }

        public override BoundNodeKind Kind => BoundNodeKind.AssignmentExpression;
        public override NamedTypeSymbol NamedType => Expression.NamedType;
        public LocalSymbol Local { get; }
        public BoundExpression Expression { get; }
    }
}
