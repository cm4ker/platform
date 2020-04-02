using dnlib.DotNet;

namespace ZenPlatform.Compiler.Roslyn.DnlibBackend
{
    internal static class SreExtensions
    {
        public static ITypeDefOrRef ToTypeRef(this SreType type)
        {
            return ((SreType) type).TypeRef;
        }

        public static ITypeDefOrRef ToTypeRef(this TypeDef type)
        {
            return new TypeRefUser(type.Module, type.Namespace, type.Name);
        }

        public static ITypeDefOrRef ToTypeRef(this ITypeDefOrRef type)
        {
            return new TypeRefUser(type.Module, type.Namespace, type.Name);
        }
    }
}