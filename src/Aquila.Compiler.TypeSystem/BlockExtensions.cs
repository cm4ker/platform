using Aquila.Compiler.Contracts;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua
{
    public static class BlockExtensions
    {
        public static IEmitter IsInst(this IEmitter bb, IPType type)
        {
            return bb.IsInst(type.ToBackend());
        }

        public static IEmitter NewObj(this IEmitter bb, IPConstructor constructor)
        {
            return bb.NewObj(constructor.ToBackend());
        }

        public static IEmitter GetClrPrivateType(IPType type)
        {
        }
    }
}