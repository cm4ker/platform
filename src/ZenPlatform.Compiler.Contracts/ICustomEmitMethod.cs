namespace ZenPlatform.Compiler.Contracts
{
    public interface ICustomEmitMethod : IMethod
    {
        void EmitCall(IEmitter emitter);
    }
}