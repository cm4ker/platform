namespace Aquila.Language.Ast.Binding
{
    internal enum BoundBinaryOperatorKind
    {
        Addition,
        Subtraction,
        Multiplication,
        Division,
        LogicalAnd,
        LogicalOr,
        BitwiseAnd,
        BitwiseOr,
        BitwiseXor,
        Equals,
        NotEquals,
        Less,
        LessOrEquals,
        Greater,
        GreaterOrEquals,
    }
}
