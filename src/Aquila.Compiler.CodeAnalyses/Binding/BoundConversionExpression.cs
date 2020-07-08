using Aquila.Language.Ast.Symbols;

namespace Aquila.Language.Ast.Binding
{
    internal sealed class BoundConversionExpression : BoundExpression
    {
        public BoundConversionExpression(SyntaxNode syntax, NamedTypeSymbol namedType, BoundExpression expression)
            : base(syntax)
        {
            NamedType = namedType;
            Expression = expression;
        }

        public override BoundNodeKind Kind => BoundNodeKind.ConversionExpression;
        public override NamedTypeSymbol NamedType { get; }
        public BoundExpression Expression { get; }
    }
}
