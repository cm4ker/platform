using dnlib.DotNet;

namespace ZenPlatform.Compiler.Roslyn.DnlibBackend
{
    public static class TypeExtensions
    {
        public static ITypeDefOrRef GetRef(this SreType type)
        {
            return ((SreType) type)?.TypeRef;
        }
    }
}