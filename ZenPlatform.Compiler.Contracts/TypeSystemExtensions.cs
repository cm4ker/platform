using System.Reflection;

namespace ZenPlatform.Compiler.Contracts
{
    public static class TypeSystemExtensions
    {
        public static ICustomAttributeBuilder CreateAttribute<T>(this IAssembly ab, params IType[] args)
        {
            var type = ab.TypeSystem.FindType<T>();
            return ab.TypeSystem.Factory.CreateAttribute(ab.TypeSystem, type, args);
        }

        public static ITypeBuilder DefineInstanceType(this IAssemblyBuilder asm, string @namespace, string name,
            IType baseType = null)
        {
            return asm.DefineType(@namespace, name, TypeAttributes.Class
                                                    | TypeAttributes.NotPublic
                                                    | TypeAttributes.BeforeFieldInit
                                                    | TypeAttributes.AnsiClass,
                (baseType == null) ? asm.TypeSystem.GetSystemBindings().Object : baseType);
        }

        public static ITypeBuilder DefineStaticType(this IAssemblyBuilder asm, string @namespace, string name)
        {
            return asm.DefineType(@namespace, name, TypeAttributes.Class
                                                    | TypeAttributes.Public | TypeAttributes.Abstract
                                                    | TypeAttributes.BeforeFieldInit
                                                    | TypeAttributes.AnsiClass,
                asm.TypeSystem.GetSystemBindings().Object);
        }

        public static ICustomAttributeBuilder CreateAttribute<T>(this ITypeSystem ts, params IType[] args)
        {
            var type = ts.FindType<T>();
            return ts.Factory.CreateAttribute(ts, type, args);
        }

        public static ICustomAttributeBuilder CreateAttribute<T>(this IType ts, params IType[] args)
        {
            return ts.Assembly.CreateAttribute<T>(args);
        }
    }
}