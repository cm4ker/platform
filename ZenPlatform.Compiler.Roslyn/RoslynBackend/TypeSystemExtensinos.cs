using System;

namespace ZenPlatform.Compiler.Roslyn.RoslynBackend
{
    public static class TypeSystemExtensinos
    {
        public static SystemTypeBindings GetSystemBindings(this RoslynTypeSystem ts)
        {
            return new SystemTypeBindings(ts);
        }


        public static RoslynType FindType<T>(this RoslynTypeSystem ts)
        {
            return FindType(ts, typeof(T));
        }

        public static RoslynType FindType(this RoslynTypeSystem ts, Type type)
        {
            var name = type.Name;
            var @namespace = type.Namespace;
            var assembly = type.Assembly.GetName().FullName;

            if (assembly.Contains("System.Private.CoreLib"))
                assembly = ts.GetSystemBindings().MSCORLIB;

            return ts.FindType($"{@namespace}.{name}", assembly);
        }
    }
}