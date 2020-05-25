using System;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Contracts.Extensions;
using dnlib.DotNet;
using dnlib.DotNet.MD;
using IType = Aquila.Compiler.Contracts.IType;

namespace Aquila.Compiler.Roslyn.RoslynBackend
{
    public class RoslynPlatformFactory : IPlatformFactory
    {
        public IAssemblyBuilder CreateAssembly(ITypeSystem ts, string assemblyName, Version assemblyVersion)
        {
            var dnts = (RoslynTypeSystem) ts;

            var asmDef = new AssemblyDefUser(assemblyName, assemblyVersion);

            var module = new ModuleDefUser(assemblyName, Guid.NewGuid(),
                AssemblyRefUser.CreateMscorlibReferenceCLR40());

            module.Context = new ModuleContext(dnts.Resolver, new Resolver(dnts.Resolver));
            module.RuntimeVersion = MDHeaderRuntimeVersion.MS_CLR_40;

            var ca = new CustomAttribute(new MethodDefUser());

            module.Kind = ModuleKind.Dll;

            asmDef.Modules.Add(module);

            var dab = new RoslynAssemblyBuilder(dnts, asmDef);

            return (RoslynAssemblyBuilder) dnts.RegisterAssembly(dab);
            ;
        }

        public ICustomAttributeBuilder CreateAttribute(ITypeSystem ts, IType type, params IType[] args)
        {
            var c = type.FindConstructor(args) as RoslynInvokableBase;

            var a = new RoslynCustomAttributeBulder((RoslynTypeSystem) ts, c.MethodRef);

            return a;
        }
    }
}