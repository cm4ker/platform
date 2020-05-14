using System.Reflection.Emit;

namespace Aquila.Compiler.Sre
{
    class SreConstructorEmitterProvider : SreMethodEmitterProviderBase
    {
        private readonly ConstructorBuilder _cb;

        public SreConstructorEmitterProvider(ConstructorBuilder cb)
        {
            _cb = cb;
        }

        public override bool InitLocals
        {
            get => _cb.InitLocals;
            set => _cb.InitLocals = value;
        }

        public override ILGenerator Generator => _cb.GetILGenerator();
    }
}