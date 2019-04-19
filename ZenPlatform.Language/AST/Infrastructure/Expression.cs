using ZenPlatform.Language.AST.Definitions;

namespace ZenPlatform.Language.AST.Infrastructure
{
    public abstract class Expression : IAstItem
    {
        public Type Type;
    }
}