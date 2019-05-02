using ZenPlatform.Compiler.AST.Definitions;

namespace ZenPlatform.Compiler.AST.Infrastructure
{
    /// <summary>
    /// Выражение
    /// </summary>
    public abstract class Expression : AstNode
    {
        public virtual ZType Type { get; set; }
    }
}