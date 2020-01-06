using System;

namespace ZenPlatform.Compiler.Contracts
{
    public interface IAssemblyPlatform
    {
        IAssemblyFactory AsmFactory { get; }

        ITypeSystem TypeSystem { get; }

        ITypeSystem CreateTypeSystem();
    }

    public static class AssemblyPlatformExtension
    {
        public static IAssemblyBuilder CreateAssembly(this IAssemblyPlatform ap, string name, Version assemblyVersion)
        {
            return ap.AsmFactory.Create(ap.TypeSystem, name, assemblyVersion);
        }

        public static IAssemblyBuilder CreateAssembly(this IAssemblyPlatform ap, string name)
        {
            return ap.AsmFactory.Create(ap.CreateTypeSystem(), name, Version.Parse("1.0.0.0"));
        }
    }
}