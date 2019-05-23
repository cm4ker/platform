namespace ZenPlatform.Compiler.Contracts
{
    public interface IMethodBuilder : IMethod
    {
        IEmitter Generator { get; }
    }
}