using Antlr4.Runtime;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.AST.Infrastructure
{
    /// <summary>
    /// Выражение
    /// </summary>
    public abstract class Expression : AstNode
    {
        public virtual IType Type { get; set; }


        protected Expression(ILineInfo li) : base(li)
        {
        }
    }
}