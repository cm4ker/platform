using ZenPlatform.Language.Ast.AST.Definitions;

namespace ZenPlatform.Language.Ast.AST.Infrastructure
{
    /// <summary>
    /// Выражение
    /// </summary>
    public abstract class Expression : AstNode
    {
        public virtual TypeNode Type { get; set; }


        protected Expression(ILineInfo li) : base(li)
        {
        }
    }
}