using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua
{
    public static class BlockExtensions
    {
        public static RoslynEmitter IsInst(this RoslynEmitter bb, IPType type)
        {
            return bb.IsInst(type.ToBackend());
        }

        public static RoslynEmitter NewObj(this RoslynEmitter bb, IPConstructor constructor)
        {
            return bb.NewObj(constructor.ToBackend());
        }

        public static RoslynType GetClrPrivateType(IPType type)
        {
        }
    }
}