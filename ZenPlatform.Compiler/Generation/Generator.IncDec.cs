using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private void EmitIncrement(IEmitter e, IType type)
        {
            EmitAddValue(e, type, 1);
        }

        private void EmitDecrement(IEmitter e, IType type)
        {
            EmitAddValue(e, type, -1);
        }
    }
}