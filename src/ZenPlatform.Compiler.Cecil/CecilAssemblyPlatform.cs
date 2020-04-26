using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Cecil
{
    public class CecilAssemblyPlatform : IAssemblyPlatform
    {
        public CecilAssemblyPlatform()
        {
            AsmFactory = new CecilPlatformFactory();
            TypeSystem = new CecilTypeSystem((CecilPlatformFactory) AsmFactory, new string[] { });
        }

        public IPlatformFactory AsmFactory { get; }

        public ITypeSystem TypeSystem { get; }

        public ITypeSystem CreateTypeSystem()
        {
            return new CecilTypeSystem((CecilPlatformFactory) AsmFactory, new string[] { });
        }
    }
}