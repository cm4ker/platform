using Aquila.Language.Ast.Symbols;

namespace Aquila.Language.Ast.Binding
{
    internal sealed class BoundVariableDeclaration : BoundStatement
    {
        public BoundVariableDeclaration(SyntaxNode syntax, LocalSymbol local, BoundExpression initializer)
            : base(syntax)
        {
            Local = local;
            Initializer = initializer;
        }

        public override BoundNodeKind Kind => BoundNodeKind.VariableDeclaration;
        public LocalSymbol Local { get; }
        public BoundExpression Initializer { get; }
    }
}
