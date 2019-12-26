using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Sre
{
    public class SreAssemblyPlatform : IAssemblyPlatform
    {
        public SreAssemblyPlatform()
        {
            AsmFactory = new SreAssemblyFactory();
            TypeSystem = new SreTypeSystem();
        }

        public IAssemblyFactory AsmFactory { get; }
        public ITypeSystem TypeSystem { get; }
        public ITypeSystem CreateTypeSystem()
        {
            return new SreTypeSystem();
        }
    }
}