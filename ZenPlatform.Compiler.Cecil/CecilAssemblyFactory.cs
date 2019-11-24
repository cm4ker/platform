using System;
using Mono.Cecil;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Cecil
{
    public class CecilAssemblyFactory : IAssemblyFactory
    {
        public IAssemblyBuilder Create(ITypeSystem ts, string assemblyName, Version assemblyVersion)
        {
            var cecilTs = (CecilTypeSystem) ts;

            var def = AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition(assemblyName, assemblyVersion),
                assemblyName,
                new ModuleParameters()
                {
                    AssemblyResolver = cecilTs,
                    MetadataResolver = cecilTs.MetadataResolver,
                    Kind = ModuleKind.Dll,
                 });

            var wrapped = (CecilAssembly) cecilTs.RegisterAssembly(def);
            
            return wrapped;
        }
    }
}