using Aquila.Language.Ast.Symbols;

namespace Aquila.Language.Ast.Binding
{
    internal abstract class BoundExpression : BoundNode
    {
        protected BoundExpression(SyntaxNode syntax)
            : base(syntax)
        {
        }

        public abstract NamedTypeSymbol NamedType { get; }
        public virtual BoundConstant? ConstantValue => null;
    }
}
