using dnlib.DotNet;

namespace Aquila.Compiler.Roslyn.RoslynBackend
{
    public static class TypeExtensions
    {
        public static ITypeDefOrRef GetRef(this RoslynType type)
        {
            return type?.TypeRef;
        }
    }
}