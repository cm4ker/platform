using System.IO;
using Aquila.Compiler.Contracts;
using dnlib.DotNet;
using IType = Aquila.Compiler.Contracts.IType;

namespace Aquila.Compiler.Roslyn.RoslynBackend
{
    internal static class TypeExtensions
    {
        public static ITypeDefOrRef GetRef(this IType type)
        {
            return ((RoslynType) type)?.TypeRef;
        }


        public static void DumpRef(this IType type, TextWriter tw)
        {
            ((RoslynType) type)?.DumpRef(tw);
        }

        public static void DumpRef(this IConstructor type, TextWriter tw)
        {
            ((RoslynConstructor) type)?.DumpRef(tw);
        }

        public static void Dump(this IParameter type, TextWriter tw)
        {
            ((RoslynParameter) type)?.Dump(tw);
        }

        public static void Dump(this IConstructor type, TextWriter tw)
        {
            ((RoslynConstructor) type)?.Dump(tw);
        }
    }
}