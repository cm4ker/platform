using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aquila.Compiler.Contracts
{
    public interface IAssemblyBuilder : IAssembly
    {
        /// <summary>
        /// Объявленные типы
        /// </summary>
        IReadOnlyList<ITypeBuilder> DefinedTypes { get; }

        ITypeBuilder DefineType(string @namespace, string name, TypeAttributes typeAttributes, IType baseType);

        void SetAttribute(ICustomAttribute attr);

        IAssembly EndBuild();
    }
}