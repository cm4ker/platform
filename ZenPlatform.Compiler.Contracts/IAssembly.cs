using System;
using System.Collections.Generic;

namespace ZenPlatform.Compiler.Contracts
{
    public interface IAssembly : IEquatable<IAssembly>
    {
        string Name { get; }
        IReadOnlyList<ICustomAttribute> CustomAttributes { get; }
        IType FindType(string fullName);

        void Write(string fileName);


        ITypeSystem TypeSystem { get; }
    }

    public interface IAssemblyFactory
    {
        IAssemblyBuilder Create(ITypeSystem ts, string assemblyName, Version assemblyVersion);
    }


    public interface ICompiler
    {
        IAssemblyFactory AsmFactory { get; }
        ITypeSystem TypeSystem { get; }
    }
}