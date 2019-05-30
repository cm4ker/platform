using System;
using System.Reflection;
using System.Reflection.Emit;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Sre
{
    public class SreAssemblyFactory : IAssemblyFactory
    {
        public IAssemblyBuilder Create(ITypeSystem ts, string assemblyName, Version assemblyVersion)
        {
            var da = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName),
                AssemblyBuilderAccess.RunAndCollect);

            return new SreAssemblyBuilder((SreTypeSystem) ts, da);
        }
    }
}