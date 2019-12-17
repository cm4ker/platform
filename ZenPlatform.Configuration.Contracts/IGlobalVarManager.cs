using ZenPlatform.Shared.Tree;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IGlobalVarManager
    {
        void Register(Node node);
    }
}