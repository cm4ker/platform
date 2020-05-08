using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Shared.Tree;

namespace Aquila.Configuration.Contracts
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