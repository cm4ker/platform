namespace ZenPlatform.Compiler.Contracts
{
    public interface IMethodBuilder : IMethod
    {
        IEmitter Generator { get; }
        IParameter DefineParameter(string name, IType type, bool isOut, bool isRef);
        IMethodBuilder WithReturnType(IType type);
    }
}