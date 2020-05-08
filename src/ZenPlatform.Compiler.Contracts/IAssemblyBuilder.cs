using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ZenPlatform.Compiler.Contracts
{
    public interface IAssemblyBuilder : IAssembly
    {
        /// <summary>
        /// Объявленные типы
        /// </summary>
        IReadOnlyList<ITypeBuilder> DefinedTypes { get; }

        ITypeBuilder DefineType(string @namespace, string name, TypeAttributes typeAttributes, IType baseType);

        /// <summary>
        /// Импортировать тип путем копирования его в сборку
        /// </summary>
        /// <param name="type"></param>
        ITypeBuilder ImportWithCopy(IType type);

        void SetAttribute(ICustomAttribute attr);

        IAssembly EndBuild();
    }
}