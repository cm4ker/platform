using dnlib.DotNet.MD;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Dnlib
{
    public class DnlibAssemblyPlatform : IAssemblyPlatform
    {
        public DnlibAssemblyPlatform()
        {
            AsmFactory = new DnlibAssemblyFactory();
            TypeSystem = new DnlibTypeSystem(new string[] { });
        }

        public IAssemblyFactory AsmFactory { get; }
        public ITypeSystem TypeSystem { get; }
    }
}