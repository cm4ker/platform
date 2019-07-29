using System;

namespace ZenPlatform.Compiler.Contracts
{
    public interface IAssemblyPlatform
    {
        IAssemblyFactory AsmFactory { get; }

        ITypeSystem TypeSystem { get; }
    }

    public static class AssemblyPlatformExtension
    {
        public static IAssemblyBuilder CreateAssembly(this IAssemblyPlatform ap, string name, Version assemblyVersion)
        {
            return ap.AsmFactory.Create(ap.TypeSystem, name, assemblyVersion);
        }
    }
}