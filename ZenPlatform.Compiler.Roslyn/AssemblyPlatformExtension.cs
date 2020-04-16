using System;
using ZenPlatform.Compiler.Roslyn.DnlibBackend;

namespace ZenPlatform.Compiler.Roslyn
{
    public static class AssemblyPlatformExtension
    {
        public static RoslynAssemblyBuilder CreateAssembly(this RoslynAssemblyPlatform ap, string name, Version assemblyVersion)
        {
            return ap.AsmFactory.CreateAssembly(ap.TypeSystem, name, assemblyVersion);
        }

        public static RoslynAssemblyBuilder CreateAssembly(this RoslynAssemblyPlatform ap, string name)
        {
            return ap.AsmFactory.CreateAssembly(ap.CreateTypeSystem(), name, Version.Parse("1.0.0.0"));
        }
    }
}