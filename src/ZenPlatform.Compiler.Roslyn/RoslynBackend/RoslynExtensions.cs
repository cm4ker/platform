using dnlib.DotNet;

namespace ZenPlatform.Compiler.Roslyn.RoslynBackend
{
    internal static class RoslynExtensions
    {
        public static ITypeDefOrRef ToTypeRef(this RoslynType type)
        {
            return ((RoslynType) type).TypeRef;
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