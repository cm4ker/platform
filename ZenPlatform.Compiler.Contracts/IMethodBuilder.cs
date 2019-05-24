namespace ZenPlatform.Compiler.Contracts
{
    public interface IMethodBuilder : IMethod
    {
        IEmitter Generator { get; }
        IMethodBuilder WithParameter(string name, IType type, bool isOut, bool isRef);
        IMethodBuilder WithReturnType(IType type);
    }
}