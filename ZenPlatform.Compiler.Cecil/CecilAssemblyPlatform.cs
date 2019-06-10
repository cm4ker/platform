using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Cecil
{
    public class CecilAssemblyPlatform : IAssemblyPlatform
    {
        public CecilAssemblyPlatform()
        {
            AsmFactory = new CecilAssemblyFactory();
            TypeSystem = new CecilTypeSystem(new string[] { });
        }

        public IAssemblyFactory AsmFactory { get; }
        public ITypeSystem TypeSystem { get; }
    }
}