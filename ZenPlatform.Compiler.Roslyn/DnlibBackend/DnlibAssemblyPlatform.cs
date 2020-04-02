namespace ZenPlatform.Compiler.Roslyn.DnlibBackend
{
    public class RoslynAssemblyPlatform
    {
        public RoslynAssemblyPlatform()
        {
            AsmFactory = new SrePlatformFactory();
            TypeSystem = new SreTypeSystem(AsmFactory, new string[] { });
        }

        public SrePlatformFactory AsmFactory { get; }

        public SreTypeSystem TypeSystem { get; }

        public SreTypeSystem CreateTypeSystem()
        {
            return new SreTypeSystem(AsmFactory, new string[] { });
        }
    }
}