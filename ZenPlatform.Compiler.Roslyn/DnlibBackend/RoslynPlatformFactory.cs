using System;
using dnlib.DotNet;
using dnlib.DotNet.MD;

namespace ZenPlatform.Compiler.Roslyn.DnlibBackend
{
    public class RoslynPlatformFactory
    {
        public RoslynAssemblyBuilder CreateAssembly(RoslynTypeSystem ts, string assemblyName, Version assemblyVersion)
        {
            var dnts = ts;

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

        public RoslynCustomAttributeBulder CreateAttribute(RoslynTypeSystem ts, RoslynType type, params RoslynType[] args)
        {
            var c = type.FindConstructor(args) as RoslynInvokableBase;

            var a = new RoslynCustomAttributeBulder((RoslynTypeSystem) ts, c.MethodRef);

            return a;
        }
    }
}