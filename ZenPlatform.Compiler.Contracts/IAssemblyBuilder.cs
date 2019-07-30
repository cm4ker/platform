using System.Collections.Generic;
using System.Reflection;

namespace ZenPlatform.Compiler.Contracts
{
    public interface IAssemblyBuilder : IAssembly
    {
        IReadOnlyList<ITypeBuilder> DefinedTypes { get; }

        ITypeBuilder DefineType(string @namespace, string name, TypeAttributes typeAttributes, IType baseType);

        //ITypeBuilder DefineType(string @namespace, string name, IType baseType);

        IAssembly EndBuild();
    }
}