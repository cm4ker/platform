using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Cecil
{
    public class CecilWellKnownTypes : IWellKnownTypes
    {
        public CecilWellKnownTypes(CecilTypeSystem ts)
        {
        }

        public IType Int { get; }
        public IType String { get; }
        public IType Object { get; }
        public IType Char { get; }
        public IType Boolean { get; }
        public IType Double { get; }
        public IType Guid { get; }
        public IType Void { get; }
        public IType Byte { get; }
        public IType DateTime { get; }
    }
}