using System;

namespace ZenPlatform.Compiler.Contracts
{
    public interface IAssemblyFactory
    {
        IAssemblyBuilder Create(ITypeSystem ts, string assemblyName, Version assemblyVersion);
    }
}