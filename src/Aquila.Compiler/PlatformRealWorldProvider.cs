using Aquila.Compiler.Roslyn;
using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Core.Contracts.TypeSystem;
using Aquila.Core.Network;

namespace Aquila.Compiler
{
    public class PlatformRealWorldProvider
    {
        public PlatformRealWorldProvider()
        {
        }
    }
    
    public class EmitterProvider<T>
    {
        public RBlockBuilder NewObj(RBlockBuilder bb, T type)
        {
            return bb.NewObj(null);
        }

        public static RoslynType GetClrPrivateType(IPType type)
        {
        }
    }
    
    
    
}