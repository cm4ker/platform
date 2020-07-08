using Aquila.Language.Ast.Symbols;

namespace Aquila.Language.Ast.Binding
{
    // TODO: Should the error expression accept an array of bound nodes so that we don't drop
    //       parts of the bound tree on the floor?

    internal sealed class BoundErrorExpression : BoundExpression
    {
        public BoundErrorExpression(SyntaxNode syntax)
            : base(syntax)
        {
        }

        public override BoundNodeKind Kind => BoundNodeKind.ErrorExpression;
        public override NamedTypeSymbol NamedType => null;
    }
}
