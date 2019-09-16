using System;
using System.Collections.Generic;

namespace ZenPlatform.Compiler.Contracts
{
    public interface ITypeSystem
    {
        IWellKnownTypes WellKnownTypes { get; }

        IReadOnlyList<IAssembly> Assemblies { get; }

        IAssembly FindAssembly(string substring);

        /// <summary>
        /// Find type in the registered in type system assemblies
        /// </summary>
        /// <param name="name">Full type name</param>
        /// <returns></returns>
        IType FindType(string name);

        /// <summary>
        /// Lookup type
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        IType FindType(string name, string assembly);
    }

    public static class TypeSystemExtensinos
    {
        public static SystemTypeBindings GetSystemBindings(this ITypeSystem ts)
        {
            return new SystemTypeBindings(ts);
        }


        public static IType FindType<T>(this ITypeSystem ts)
        {
            return FindType(ts, typeof(T));
        }

        public static IType FindType(this ITypeSystem ts, Type type)
        {
            var name = type.Name;
            var @namespace = type.Namespace;
            var assembly = type.Assembly.GetName().FullName;

            return ts.FindType($"{@namespace}.{name}", assembly);
        }
    }
}