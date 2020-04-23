namespace ZenPlatform.Compiler.Roslyn.RoslynBackend
{
    public class RoslynAssemblyPlatform
    {
        public RoslynAssemblyPlatform()
        {
            AsmFactory = new RoslynPlatformFactory();
            TypeSystem = new RoslynTypeSystem(AsmFactory, new string[] { });
        }

        public RoslynPlatformFactory AsmFactory { get; }

        public RoslynTypeSystem TypeSystem { get; }

        public RoslynTypeSystem CreateTypeSystem()
        {
            return new RoslynTypeSystem(AsmFactory, new string[] { });
        }
    }
}