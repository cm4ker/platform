using System;
using System.Reflection;
using ZenPlatform.Compiler.Roslyn.DnlibBackend;

namespace ZenPlatform.Compiler.Roslyn
{
    public static class TypeSystemExtensions2
    {
        public static SreCustomAttributeBulder CreateAttribute<T>(this SreAssembly ab, params SreType[] args)
        {
            var type = ab.TypeSystem.Resolve<T>();
            return ab.TypeSystem.Factory.CreateAttribute(ab.TypeSystem, type, args);
        }

        public static SreTypeBuilder DefineInstanceType(this SreAssemblyBuilder asm, string @namespace, string name,
            SreType baseType = null)
        {
            return asm.DefineType(@namespace, name, TypeAttributes.Class
                                                    | TypeAttributes.Public
                                                    | TypeAttributes.BeforeFieldInit
                                                    | TypeAttributes.AnsiClass,
                baseType ?? asm.TypeSystem.GetSystemBindings().Object);
        }

        public static SreTypeBuilder DefineStaticType(this SreAssemblyBuilder asm, string @namespace, string name,
            SreType baseType = null)
        {
            return asm.DefineType(@namespace, name, TypeAttributes.Class
                                                    | TypeAttributes.Public | TypeAttributes.Abstract
                                                    | TypeAttributes.BeforeFieldInit
                                                    | TypeAttributes.AnsiClass,
                baseType ?? asm.TypeSystem.GetSystemBindings().Object);
        }

        public static SreCustomAttributeBulder CreateAttribute<T>(this SreTypeSystem ts, params SreType[] args)
        {
            var type = ts.Resolve<T>();
            return ts.Factory.CreateAttribute(ts, type, args);
        }

        public static SreCustomAttributeBulder CreateAttribute<T>(this SreType ts, params SreType[] args)
        {
            return ts.Assembly.CreateAttribute<T>(args);
        }
    }
    
    public static class AssemblyPlatformExtension
    {
        public static SreAssemblyBuilder CreateAssembly(this RoslynAssemblyPlatform ap, string name, Version assemblyVersion)
        {
            return ap.AsmFactory.CreateAssembly(ap.TypeSystem, name, assemblyVersion);
        }

        public static SreAssemblyBuilder CreateAssembly(this RoslynAssemblyPlatform ap, string name)
        {
            return ap.AsmFactory.CreateAssembly(ap.CreateTypeSystem(), name, Version.Parse("1.0.0.0"));
        }
    }
}