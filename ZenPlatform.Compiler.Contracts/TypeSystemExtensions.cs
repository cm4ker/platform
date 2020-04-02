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

        // public static ITypeBuilder DefineInstanceType(this IAssemblyBuilder asm, string @namespace, string name,
        //     IType baseType = null)
        // {
        //     return asm.DefineType(@namespace, name, TypeAttributes.Class
        //                                             | TypeAttributes.Public
        //                                             | TypeAttributes.BeforeFieldInit
        //                                             | TypeAttributes.AnsiClass,
        //         baseType ?? asm.TypeSystem.GetSystemBindings().Object);
        // }

        public static ITypeBuilder DefineStaticType(this IAssemblyBuilder asm, string @namespace, string name,
            IType baseType = null)
        {
            return asm.DefineType(@namespace, name, TypeAttributes.Class
                                                    | TypeAttributes.Public | TypeAttributes.Abstract
                                                    | TypeAttributes.BeforeFieldInit
                                                    | TypeAttributes.AnsiClass,
                baseType ?? asm.TypeSystem.GetSystemBindings().Object);
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