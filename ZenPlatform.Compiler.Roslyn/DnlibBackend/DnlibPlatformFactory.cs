using System;
using dnlib.DotNet;
using dnlib.DotNet.MD;

namespace ZenPlatform.Compiler.Roslyn.DnlibBackend
{
    public class SrePlatformFactory
    {
        public SreAssemblyBuilder CreateAssembly(SreTypeSystem ts, string assemblyName, Version assemblyVersion)
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

            var dab = new SreAssemblyBuilder(dnts, asmDef);

            return (SreAssemblyBuilder) dnts.RegisterAssembly(dab);
            ;
        }

        public SreCustomAttributeBulder CreateAttribute(SreTypeSystem ts, SreType type, params SreType[] args)
        {
            var c = type.FindConstructor(args) as SreInvokableBase;

            var a = new SreCustomAttributeBulder((SreTypeSystem) ts, c.MethodRef);

            return a;
        }
    }
}