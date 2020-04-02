using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Roslyn.DnlibBackend;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IGlobalVarManager
    {
        SreTypeSystem TypeSystem { get; }

        void Register(Node node);

        Node Root { get; }
    }

    public interface IEntryPointManager
    {
        SreTypeBuilder EntryPoint { get; }
        SreMethodBuilder Main { get; }
    }
}