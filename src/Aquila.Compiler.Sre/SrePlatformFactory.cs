using System;
using System.Reflection;
using System.Reflection.Emit;
using Aquila.Compiler.Contracts;

namespace Aquila.Compiler.Sre
{
    public class SrePlatformFactory : IPlatformFactory
    {
        public IAssemblyBuilder CreateAssembly(ITypeSystem ts, string assemblyName, Version assemblyVersion)
        {
            var da = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName),
                AssemblyBuilderAccess.Run);

            return new SreAssemblyBuilder((SreTypeSystem) ts, da);
        }

        public ICustomAttributeBuilder CreateAttribute(ITypeSystem ts, IType type, params IType[] args)
        {
            throw new NotImplementedException();
        }
    }
}