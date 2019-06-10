namespace ZenPlatform.Compiler.Contracts
{
    public interface IAssemblyPlatform
    {
        IAssemblyFactory AsmFactory { get; }
        ITypeSystem TypeSystem { get; }
    }
}