using dnlib.DotNet.MD;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Dnlib
{
    public class DnlibAssemblyPlatform : IAssemblyPlatform
    {
        public IAssemblyFactory AsmFactory { get; }
        public ITypeSystem TypeSystem { get; }
    }
}