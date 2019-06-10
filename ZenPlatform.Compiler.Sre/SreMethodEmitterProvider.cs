using System.Reflection.Emit;

namespace ZenPlatform.Compiler.Sre
{
    class SreMethodEmitterProvider : SreMethodEmitterProviderBase
    {
        private readonly MethodBuilder _mb;

        public SreMethodEmitterProvider(MethodBuilder mb)
        {
            _mb = mb;
        }

        public override bool InitLocals
        {
            get => _mb.InitLocals;
            set => _mb.InitLocals = value;
        }

        public override ILGenerator Generator => _mb.GetILGenerator();
    }
}