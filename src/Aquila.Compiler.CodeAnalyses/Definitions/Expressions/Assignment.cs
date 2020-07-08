namespace Aquila.Language.Ast.Definitions.Expressions
{
    public partial class GlobalVar
    {
        public override Ast.TypeSyntax Type
        {
            get => Expression.Type;
        }
    }

    /// <summary>
    /// Выражение для постинкрементирования
    /// </summary>
    public partial class PostIncrementExpression
    {
        public override Ast.TypeSyntax Type => Expression.Type;
    }

    public partial class PostDecrementExpression
    {
        public override Ast.TypeSyntax Type => Expression.Type;
    }
}