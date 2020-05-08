using System;
using Mono.Cecil;
using Aquila.Compiler.Contracts;

namespace Aquila.Compiler.Cecil
{
    public class CecilPlatformFactory : IPlatformFactory
    {
        public IAssemblyBuilder CreateAssembly(ITypeSystem ts, string assemblyName, Version assemblyVersion)
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

        public ICustomAttributeBuilder CreateAttribute(ITypeSystem ts, IType type, params IType[] args)
        {
            throw new NotImplementedException();
        }

        public ICustomAttributeBuilder CreateAttribute(IType type, params IType[] args)
        {
            throw new NotImplementedException();
        }
    }
}