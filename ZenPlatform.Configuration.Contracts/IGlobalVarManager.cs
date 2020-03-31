using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IGlobalVarManager
    {
        ITypeSystem TypeSystem { get; }

        void Register(Node node);

        Node Root { get; }
    }

    public interface IAssemblyServiceManager
    {
        ITypeBuilder ServiceInitializerType { get; }
        IMethodBuilder ServiceInitializerInitMethod { get; }
        IConstructorBuilder ServiceInitializerConstructor { get; }
        IField InvokeServiceField { get; }
    }
}