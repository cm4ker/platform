using dnlib.DotNet;

namespace ZenPlatform.Compiler.Roslyn.RoslynBackend
{
    public static class TypeExtensions
    {
        public static ITypeDefOrRef GetRef(this RoslynType type)
        {
            return type?.TypeRef;
        }
    }
}