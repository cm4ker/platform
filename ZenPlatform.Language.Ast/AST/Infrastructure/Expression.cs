using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Definitions;
using ZenPlatform.Language.Ast.AST.Definitions.Functions;

namespace ZenPlatform.Language.Ast.AST.Infrastructure
{
    /// <summary>
    /// Выражение
    /// </summary>
    public abstract class Expression : AstNode, ITypedNode
    {
        public virtual TypeNode Type { get; set; }


        protected Expression(ILineInfo li) : base(li)
        {
        }
    }
}