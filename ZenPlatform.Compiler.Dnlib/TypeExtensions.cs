using dnlib.DotNet;
using Microsoft.VisualBasic.CompilerServices;
using IType = ZenPlatform.Compiler.Contracts.IType;

namespace ZenPlatform.Compiler.Dnlib
{
    public static class TypeExtensions
    {
        public static ITypeDefOrRef GetRef(this IType type)
        {
            return ((DnlibType) type)?.TypeRef;
        }
    }
}