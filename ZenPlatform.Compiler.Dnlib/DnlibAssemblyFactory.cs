using System;
using dnlib.DotNet;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Dnlib
{
    public class DnlibAssemblyFactory : IAssemblyFactory
    {
        public IAssemblyBuilder Create(ITypeSystem ts, string assemblyName, Version assemblyVersion)
        {
            var dnts = (DnlibTypeSystem) ts;


            var asmDef = new AssemblyDefUser(assemblyName, assemblyVersion);

            return new DnlibAssemblyBuilder(dnts, asmDef);
        }
    }
}