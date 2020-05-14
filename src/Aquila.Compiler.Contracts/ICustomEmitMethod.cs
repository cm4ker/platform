namespace Aquila.Compiler.Contracts
{
    public interface ICustomEmitMethod : IMethod
    {
        void EmitCall(IEmitter emitter);
    }
}