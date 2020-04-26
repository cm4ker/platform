using ZenPlatform.Compiler.Contracts.Symbols;

namespace ZenPlatform.Language.Ast.Definitions.Expressions
{
    public partial class GlobalVar
    {
        public override TypeSyntax Type
        {
            get => Expression.Type;
        }
    }

    public partial class Assignment : Expression
    {
        public override TypeSyntax Type
        {
            get
            {
                if (Assignable is Name n) return n.Type;
                else return null;
            }
        }
    }

    /// <summary>
    /// Выражение для постинкрементирования
    /// </summary>
    public partial class PostIncrementExpression
    {
        public override TypeSyntax Type => Expression.Type;
    }

    public partial class PostDecrementExpression
    {
        public override TypeSyntax Type => Expression.Type;
    }
}