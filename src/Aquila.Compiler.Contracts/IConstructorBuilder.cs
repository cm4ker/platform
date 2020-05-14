namespace Aquila.Compiler.Contracts
{
    public interface IConstructorBuilder : IConstructor
    {
        IEmitter Generator { get; }

        IParameter DefineParameter(IType type);
    }
}