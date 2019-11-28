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
            var module = new ModuleDefUser(assemblyName, Guid.NewGuid(),
                AssemblyRefUser.CreateMscorlibReferenceCLR40());
            
            module.Kind = ModuleKind.Dll;
            asmDef.Modules.Add(module);

            var dab = new DnlibAssemblyBuilder(dnts, asmDef);
            return (IAssemblyBuilder) dnts.RegisterAssembly(dab);
            ;
        }
    }
}