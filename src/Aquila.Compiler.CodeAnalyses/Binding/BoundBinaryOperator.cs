using Aquila.Language.Ast.Infrastructure;
using Aquila.Language.Ast.Symbols;


namespace Aquila.Language.Ast.Binding
{
    internal sealed class BoundBinaryOperator
    {
        // private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, NamedTypeSymbol namedType)
        //     : this(syntaxKind, kind, namedType, namedType, namedType)
        // {
        // }
        //
        // private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind,
        //     NamedTypeSymbol operandNamedType, NamedTypeSymbol resultNamedType)
        //     : this(syntaxKind, kind, operandNamedType, operandNamedType, resultNamedType)
        // {
        // }

        private BoundBinaryOperator(BoundBinaryOperatorKind kind, NamedTypeSymbol leftNamedType,
            NamedTypeSymbol rightNamedType, NamedTypeSymbol resultNamedType)
        {
            Kind = kind;
            LeftNamedType = leftNamedType;
            RightNamedType = rightNamedType;
            NamedType = resultNamedType;
        }

        // public SyntaxKind SyntaxKind { get; }
        public BoundBinaryOperatorKind Kind { get; }
        public NamedTypeSymbol LeftNamedType { get; }
        public NamedTypeSymbol RightNamedType { get; }
        public NamedTypeSymbol NamedType { get; }

        private static BoundBinaryOperator[] _operators =
        {
            // new BoundBinaryOperator(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition, SpecialType.System_Int32),
            // new BoundBinaryOperator(SyntaxKind.MinusToken, BoundBinaryOperatorKind.Subtraction, NamedTypeSymbol.Int),
            // new BoundBinaryOperator(SyntaxKind.StarToken, BoundBinaryOperatorKind.Multiplication, NamedTypeSymbol.Int),
            // new BoundBinaryOperator(SyntaxKind.SlashToken, BoundBinaryOperatorKind.Division, NamedTypeSymbol.Int),
            // new BoundBinaryOperator(SyntaxKind.AmpersandToken, BoundBinaryOperatorKind.BitwiseAnd, NamedTypeSymbol.Int),
            // new BoundBinaryOperator(SyntaxKind.PipeToken, BoundBinaryOperatorKind.BitwiseOr, NamedTypeSymbol.Int),
            // new BoundBinaryOperator(SyntaxKind.HatToken, BoundBinaryOperatorKind.BitwiseXor, NamedTypeSymbol.Int),
            // new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.Equals, NamedTypeSymbol.Int,
            //     NamedTypeSymbol.Bool),
            // new BoundBinaryOperator(SyntaxKind.BangEqualsToken, BoundBinaryOperatorKind.NotEquals, NamedTypeSymbol.Int,
            //     NamedTypeSymbol.Bool),
            // new BoundBinaryOperator(SyntaxKind.LessToken, BoundBinaryOperatorKind.Less, NamedTypeSymbol.Int,
            //     NamedTypeSymbol.Bool),
            // new BoundBinaryOperator(SyntaxKind.LessOrEqualsToken, BoundBinaryOperatorKind.LessOrEquals,
            //     NamedTypeSymbol.Int, NamedTypeSymbol.Bool),
            // new BoundBinaryOperator(SyntaxKind.GreaterToken, BoundBinaryOperatorKind.Greater, NamedTypeSymbol.Int,
            //     NamedTypeSymbol.Bool),
            // new BoundBinaryOperator(SyntaxKind.GreaterOrEqualsToken, BoundBinaryOperatorKind.GreaterOrEquals,
            //     NamedTypeSymbol.Int, NamedTypeSymbol.Bool),
            //
            // new BoundBinaryOperator(SyntaxKind.AmpersandToken, BoundBinaryOperatorKind.BitwiseAnd,
            //     NamedTypeSymbol.Bool),
            // new BoundBinaryOperator(SyntaxKind.AmpersandAmpersandToken, BoundBinaryOperatorKind.LogicalAnd,
            //     NamedTypeSymbol.Bool),
            // new BoundBinaryOperator(SyntaxKind.PipeToken, BoundBinaryOperatorKind.BitwiseOr, NamedTypeSymbol.Bool),
            // new BoundBinaryOperator(SyntaxKind.PipePipeToken, BoundBinaryOperatorKind.LogicalOr, NamedTypeSymbol.Bool),
            // new BoundBinaryOperator(SyntaxKind.HatToken, BoundBinaryOperatorKind.BitwiseXor, NamedTypeSymbol.Bool),
            // new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.Equals, NamedTypeSymbol.Bool),
            // new BoundBinaryOperator(SyntaxKind.BangEqualsToken, BoundBinaryOperatorKind.NotEquals,
            //     NamedTypeSymbol.Bool),
            //
            // new BoundBinaryOperator(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition, NamedTypeSymbol.String),
            // new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.Equals,
            //     NamedTypeSymbol.String, NamedTypeSymbol.Bool),
            // new BoundBinaryOperator(SyntaxKind.BangEqualsToken, BoundBinaryOperatorKind.NotEquals,
            //     NamedTypeSymbol.String, NamedTypeSymbol.Bool),
            //
            // new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.Equals, NamedTypeSymbol.Any),
            // new BoundBinaryOperator(SyntaxKind.BangEqualsToken, BoundBinaryOperatorKind.NotEquals, NamedTypeSymbol.Any)
        };

        public static BoundBinaryOperator? Bind(BinaryOperatorType syntaxKind, NamedTypeSymbol leftNamedType,
            NamedTypeSymbol rightNamedType)
        {
            // foreach (var op in _operators)
            // {
            //     if (op.SyntaxKind == syntaxKind && op.LeftNamedType == leftNamedType &&
            //         op.RightNamedType == rightNamedType)
            //         return op;
            // }
            //
            return null;
        }
    }
}