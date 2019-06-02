using System.Reflection.Emit;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Sre
{
    class SreLocal : ILocal
    {
        public LocalBuilder Local { get; }

        public SreLocal(LocalBuilder local)
        {
            Local = local;
        }
    }
}