using Aquila.Core.Contracts.TypeSystem;
using Aquila.Shared.Tree;

namespace Aquila.Core.Contracts.Configuration
{
    public interface IGlobalVarManager
    {
        ITypeManager TypeSystem { get; }

        void Register(Node node);

        Node Root { get; }
    }

    public interface IEntryPointManager
    {
        RoslynTypeBuilder EntryPoint { get; }
        RoslynMethodBuilder Main { get; }
    }
}