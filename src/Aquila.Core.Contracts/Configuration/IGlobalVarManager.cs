using Aquila.Compiler.Aqua.TypeSystem;
using Aquila.Compiler.Aqua.TypeSystem.Builders;
using Aquila.Core.Contracts.TypeSystem;
using Aquila.Shared.Tree;

namespace Aquila.Core.Contracts.Configuration
{
    public interface IGlobalVarManager
    {
        TypeManager TypeSystem { get; }

        void Register(Node node);

        Node Root { get; }
    }

    public interface IEntryPointManager
    {
        PTypeBuilder EntryPoint { get; }
        PMethodBuilder Main { get; }
    }
}