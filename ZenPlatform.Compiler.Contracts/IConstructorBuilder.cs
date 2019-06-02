namespace ZenPlatform.Compiler.Contracts
{
    public interface IConstructorBuilder : IConstructor
    {
        IEmitter Generator { get; }
    }
}