using Aquila.Language.Ast.Infrastructure;
using Aquila.Language.Ast.Symbols;
using Microsoft.CodeAnalysis.Operations;

namespace Aquila.Language.Ast.Binding
{
    internal sealed class BoundUnaryOperator
    {
        private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, NamedTypeSymbol operandNamedType)
            : this(syntaxKind, kind, operandNamedType, operandNamedType)
        {
        }

        private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, NamedTypeSymbol operandNamedType, NamedTypeSymbol resultNamedType)
        {
            SyntaxKind = syntaxKind;
            Kind = kind;
            OperandNamedType = operandNamedType;
            NamedType = resultNamedType;
        }

        public UnaryOperatorType SyntaxKind { get; }
        public BoundUnaryOperatorKind Kind { get; }
        public NamedTypeSymbol OperandNamedType { get; }
        public NamedTypeSymbol NamedType { get; }

        private static BoundUnaryOperator[] _operators =
        {
            // new BoundUnaryOperator(SyntaxKind.BangToken, BoundUnaryOperatorKind.LogicalNegation, NamedTypeSymbol.Bool),
            //
            // new BoundUnaryOperator(SyntaxKind.PlusToken, BoundUnaryOperatorKind.Identity, NamedTypeSymbol.Int),
            // new BoundUnaryOperator(SyntaxKind.MinusToken, BoundUnaryOperatorKind.Negation, NamedTypeSymbol.Int),
            // new BoundUnaryOperator(SyntaxKind.TildeToken, BoundUnaryOperatorKind.OnesComplement, NamedTypeSymbol.Int),
        };

        public static BoundUnaryOperator? Bind(UnaryOperatorType syntaxKind, NamedTypeSymbol operandNamedType)
        {
            foreach (var op in _operators)
            {
                if (op.SyntaxKind == syntaxKind && op.OperandNamedType == operandNamedType)
                    return op;
            }

            return null;
        }
    }
}
