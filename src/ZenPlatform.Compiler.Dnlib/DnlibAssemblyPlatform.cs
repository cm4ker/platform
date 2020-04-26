using dnlib.DotNet.MD;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Dnlib
{
    public class DnlibAssemblyPlatform : IAssemblyPlatform
    {
        public DnlibAssemblyPlatform()
        {
            AsmFactory = new DnlibPlatformFactory();
            TypeSystem = new DnlibTypeSystem((DnlibPlatformFactory) AsmFactory, new string[] { });
        }

        public IPlatformFactory AsmFactory { get; }

        public ITypeSystem TypeSystem { get; }

        public ITypeSystem CreateTypeSystem()
        {
            return new DnlibTypeSystem((DnlibPlatformFactory) AsmFactory, new string[] { });
        }
    }
}