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
        IPType EntryPoint { get; }
        IPMethod Main { get; }
    }
}