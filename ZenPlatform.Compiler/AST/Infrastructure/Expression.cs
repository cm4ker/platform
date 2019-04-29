using ZenPlatform.Compiler.AST.Definitions;

namespace ZenPlatform.Compiler.AST.Infrastructure
{
    /// <summary>
    /// Выражение
    /// </summary>
    public abstract class Expression : AstNode, IAstItem
    {
        public virtual Type Type { get; set; }
    }
}