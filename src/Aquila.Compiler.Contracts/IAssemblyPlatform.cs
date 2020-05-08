using System;

namespace Aquila.Compiler.Contracts
{
    public interface IAssemblyPlatform
    {
        IPlatformFactory AsmFactory { get; }

        ITypeSystem TypeSystem { get; }

        ITypeSystem CreateTypeSystem();
    }

    public static class AssemblyPlatformExtension
    {
        public static IAssemblyBuilder CreateAssembly(this IAssemblyPlatform ap, string name, Version assemblyVersion)
        {
            return ap.AsmFactory.CreateAssembly(ap.TypeSystem, name, assemblyVersion);
        }

        public static IAssemblyBuilder CreateAssembly(this IAssemblyPlatform ap, string name)
        {
            return ap.AsmFactory.CreateAssembly(ap.CreateTypeSystem(), name, Version.Parse("1.0.0.0"));
        }
    }
}