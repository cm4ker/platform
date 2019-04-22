using ZenPlatform.Language.AST.Definitions;

namespace ZenPlatform.Language.AST.Infrastructure
{
    public abstract class Expression
    {
        public virtual Type Type { get; set; }
    }
}