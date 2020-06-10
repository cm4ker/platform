using Aquila.Compiler.Contracts;

namespace Aquila.Compiler.Roslyn.RoslynBackend
{
    public class RoslynAssemblyPlatform : IAssemblyPlatform
    {
        public RoslynAssemblyPlatform()
        {
            AsmFactory = new RoslynPlatformFactory();
            TypeSystem = new RoslynTypeSystem(AsmFactory, new string[] { });
        }

        public IPlatformFactory AsmFactory { get; }

        public ITypeSystem TypeSystem { get; }

        public ITypeSystem CreateTypeSystem()
        {
            return new RoslynTypeSystem(AsmFactory, new string[] { });
        }
    }
}