using dnlib.DotNet;
using Microsoft.VisualBasic.CompilerServices;
using IType = Aquila.Compiler.Contracts.IType;

namespace Aquila.Compiler.Dnlib
{
    public static class TypeExtensions
    {
        public static ITypeDefOrRef GetRef(this IType type)
        {
            return ((DnlibType) type)?.TypeRef;
        }
    }
}