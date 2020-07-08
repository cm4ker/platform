using System.Collections.Immutable;
using Aquila.Language.Ast.Symbols;

namespace Aquila.Language.Ast.Binding
{
    internal sealed class BoundCallExpression : BoundExpression
    {
        public BoundCallExpression(SyntaxNode syntax, BoundExpression expression, MethodSymbol method,
            ImmutableArray<BoundExpression> arguments)
            : base(syntax)
        {
            Method = method;
            Arguments = arguments;
        }

        /// <summary>
        /// Expression
        /// </summary>
        public BoundExpression Expression { get; }

        public MethodSymbol Method { get; }
        public override BoundNodeKind Kind => BoundNodeKind.CallExpression;
        public override NamedTypeSymbol NamedType => Method.NamedType;

        public ImmutableArray<BoundExpression> Arguments { get; }
    }
}