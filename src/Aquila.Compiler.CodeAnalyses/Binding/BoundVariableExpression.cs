using Aquila.Language.Ast.Symbols;

namespace Aquila.Language.Ast.Binding
{
    internal sealed class BoundVariableExpression : BoundExpression
    {
        public BoundVariableExpression(SyntaxNode syntax, LocalSymbol local)
            : base(syntax)
        {
            Local = local;
        }

        public override BoundNodeKind Kind => BoundNodeKind.VariableExpression;
        public override NamedTypeSymbol NamedType => Local.NamedType;
        public LocalSymbol Local { get; }
        public override BoundConstant? ConstantValue => Local.Constant;
    }
}
