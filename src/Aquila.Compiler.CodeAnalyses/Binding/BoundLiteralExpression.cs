using System;
using Aquila.Language.Ast.Symbols;

namespace Aquila.Language.Ast.Binding
{
    internal sealed class BoundLiteralExpression : BoundExpression
    {
        public BoundLiteralExpression(SyntaxNode syntax, object value)
            : base(syntax)
        {

                throw new Exception($"Unexpected literal '{value}' of type {value.GetType()}");

            ConstantValue = new BoundConstant(value);
        }

        public override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;
        public override NamedTypeSymbol NamedType { get; }
        public object Value => ConstantValue.Value;
        public override BoundConstant ConstantValue { get; }
    }
}
