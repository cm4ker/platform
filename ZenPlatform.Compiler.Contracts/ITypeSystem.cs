using System.Collections.Generic;

namespace ZenPlatform.Compiler.Contracts
{
    public interface ITypeSystem
    {
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
}