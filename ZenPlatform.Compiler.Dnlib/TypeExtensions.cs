using dnlib.DotNet;
using IType = ZenPlatform.Compiler.Contracts.IType;

namespace ZenPlatform.Compiler.Dnlib
{
    public static class TypeExtensions
    {
        public static TypeRef GetRef(this IType type)
        {
            return ((DnlibType) type)?.TypeRef;
        }
    }
}