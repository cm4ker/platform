using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Sre
{
    public class SreAssemblyPlatform : IAssemblyPlatform
    {
        public SreAssemblyPlatform()
        {
            AsmFactory = new SrePlatformFactory();
            TypeSystem = new SreTypeSystem();
        }

        public IPlatformFactory AsmFactory { get; }
        public ITypeSystem TypeSystem { get; }
        public ITypeSystem CreateTypeSystem()
        {
            return new SreTypeSystem();
        }
    }
}