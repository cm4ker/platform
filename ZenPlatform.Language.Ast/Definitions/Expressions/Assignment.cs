using ZenPlatform.Compiler.Contracts.Symbols;

namespace ZenPlatform.Language.Ast.Definitions.Expressions
{
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
    public partial class PostIncrementExpression : Expression
    {
        public PostIncrementExpression(ILineInfo li, string name) : this(li, new Name(li, name))
        {
        }

        public override TypeSyntax Type => Name.Type;
    }

    public partial class PostDecrementExpression : Expression
    {
        public PostDecrementExpression(ILineInfo li, string name) : this(li, new Name(li, name))
        {
        }

        public override TypeSyntax Type => Name.Type;
    }
}