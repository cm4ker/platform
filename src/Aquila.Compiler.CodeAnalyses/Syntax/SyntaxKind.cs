namespace Aquila.Language.Ast
{
    public enum SyntaxKind
    {
        BadToken,

        // Trivia
        SkippedTextTrivia,
        LineBreakTrivia,
        WhitespaceTrivia,
        SingleLineCommentTrivia,
        MultiLineCommentTrivia,

        // Tokens
        EndOfFileToken,
        NumberToken,
        StringToken,
        PlusToken,
        PlusEqualsToken,
        MinusToken,
        MinusEqualsToken,
        StarToken,
        StarEqualsToken,
        SlashToken,
        SlashEqualsToken,
        BangToken,
        EqualsToken,
        TildeToken,
        HatToken,
        HatEqualsToken,
        AmpersandToken,
        AmpersandAmpersandToken,
        AmpersandEqualsToken,
        PipeToken,
        PipeEqualsToken,
        PipePipeToken,
        EqualsEqualsToken,
        BangEqualsToken,
        LessToken,
        LessOrEqualsToken,
        GreaterToken,
        GreaterOrEqualsToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        OpenBraceToken,
        CloseBraceToken,
        ColonToken,
        CommaToken,
        IdentifierToken,

        // Keywords
        BreakKeyword,
        ContinueKeyword,
        ElseKeyword,
        FalseKeyword,
        ForKeyword,
        FunctionKeyword,
        IfKeyword,
        LetKeyword,
        ReturnKeyword,
        ToKeyword,
        TrueKeyword,
        VarKeyword,
        WhileKeyword,
        DoKeyword,

        // Nodes
        CompilationUnit,
        MethodDeclaration,
        FieldDeclaration,
        GlobalStatement,
        Parameter,
        Type,
        ArrayType,
        ElseClause,
        Attribute,
        Argument,
        UsingDerictive,

        // Statements
        BlockStatement,
        VariableDeclaration,
        VariableDeclarator,
        IfStatement,
        WhileStatement,
        DoWhileStatement,
        ForStatement,
        BreakStatement,
        ContinueStatement,
        ReturnStatement,
        ExpressionStatement,

        // Expressions
        LiteralExpression,
        NameExpression,
        UnaryExpression,
        BinaryExpression,
        CompoundAssignmentExpression,
        ParenthesizedExpression,
        AssignmentExpression,
        CallExpression,
        IndexerExpression,
        CastExpression,
        MemberAccessExpression,
    }
}