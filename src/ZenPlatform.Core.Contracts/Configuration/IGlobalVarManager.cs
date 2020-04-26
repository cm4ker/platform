using ZenPlatform.Compiler.Roslyn.RoslynBackend;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IGlobalVarManager
    {
        RoslynTypeSystem TypeSystem { get; }

        void Register(Node node);

        Node Root { get; }
    }

    public interface IEntryPointManager
    {
        RoslynTypeBuilder EntryPoint { get; }
        RoslynMethodBuilder Main { get; }
    }
}