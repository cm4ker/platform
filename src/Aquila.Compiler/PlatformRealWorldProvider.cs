using Aquila.Compiler.Roslyn;
using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler
{
    public class PlatformRealWorldProvider
    {
        public PlatformRealWorldProvider()
        {
        }
    }


    public static class EmitterProvider
    {
        public static RBlockBuilder NewObj(this RBlockBuilder bb, IPType type)
        {
            bb.NewObj()
        }

        public static RoslynType GetClrPrivateType(IPType type)
        {
        }
    }
}