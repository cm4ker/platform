using ZenPlatform.Compiler.AST.Definitions;

namespace ZenPlatform.Compiler.AST.Infrastructure
{
    public abstract class Expression : IAstItem
    {
        public virtual Type Type { get; set; }
    }
}