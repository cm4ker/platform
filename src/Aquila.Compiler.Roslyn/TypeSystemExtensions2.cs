using System.Reflection;
using Aquila.Compiler.Roslyn.RoslynBackend;

namespace Aquila.Compiler.Roslyn
{
    public static class TypeSystemExtensions2
    {
        public static RoslynCustomAttributeBulder CreateAttribute<T>(this RoslynAssembly ab, params RoslynType[] args)
        {
            var type = ab.TypeSystem.Resolve<T>();
            return ab.TypeSystem.Factory.CreateAttribute(ab.TypeSystem, type, args);
        }

        public static RoslynTypeBuilder DefineInstanceType(this RoslynAssemblyBuilder asm, string @namespace, string name,
            RoslynType baseType = null)
        {
            return asm.DefineType(@namespace, name, TypeAttributes.Class
                                                    | TypeAttributes.Public
                                                    | TypeAttributes.BeforeFieldInit
                                                    | TypeAttributes.AnsiClass,
                baseType ?? asm.TypeSystem.GetSystemBindings().Object);
        }

        public static RoslynTypeBuilder DefineStaticType(this RoslynAssemblyBuilder asm, string @namespace, string name,
            RoslynType baseType = null)
        {
            return asm.DefineType(@namespace, name, TypeAttributes.Class
                                                    | TypeAttributes.Public | TypeAttributes.Abstract
                                                    | TypeAttributes.BeforeFieldInit
                                                    | TypeAttributes.AnsiClass,
                baseType ?? asm.TypeSystem.GetSystemBindings().Object);
        }

        public static RoslynCustomAttributeBulder CreateAttribute<T>(this RoslynTypeSystem ts, params RoslynType[] args)
        {
            var type = ts.Resolve<T>();
            return ts.Factory.CreateAttribute(ts, type, args);
        }

        public static RoslynCustomAttributeBulder CreateAttribute<T>(this RoslynType ts, params RoslynType[] args)
        {
            return ts.Assembly.CreateAttribute<T>(args);
        }
    }
}