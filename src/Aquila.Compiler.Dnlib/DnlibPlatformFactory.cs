using System;
using System.Runtime.Versioning;
using dnlib.DotNet;
using dnlib.DotNet.MD;
using dnlib.PE;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Contracts.Extensions;
using IType = Aquila.Compiler.Contracts.IType;

namespace Aquila.Compiler.Dnlib
{
    public class DnlibPlatformFactory : IPlatformFactory
    {
        public IAssemblyBuilder CreateAssembly(ITypeSystem ts, string assemblyName, Version assemblyVersion)
        {
            var dnts = (DnlibTypeSystem) ts;

            var asmDef = new AssemblyDefUser(assemblyName, assemblyVersion);

            var module = new ModuleDefUser(assemblyName, Guid.NewGuid(),
                AssemblyRefUser.CreateMscorlibReferenceCLR40());

            module.Context = new ModuleContext(dnts.Resolver, new DnlibMetadataResolver(dnts.Resolver));
            module.RuntimeVersion = MDHeaderRuntimeVersion.MS_CLR_40;

            var ca = new CustomAttribute(new MethodDefUser());

            module.Kind = ModuleKind.Dll;

            asmDef.Modules.Add(module);

            var dab = new DnlibAssemblyBuilder(dnts, asmDef);

            return (IAssemblyBuilder) dnts.RegisterAssembly(dab);
            ;
        }

        public ICustomAttributeBuilder CreateAttribute(ITypeSystem ts, IType type, params IType[] args)
        {
            var c = type.FindConstructor(args) as DnlibMethodBase;

            var a = new DnlibCustomAttributeBulder((DnlibTypeSystem) ts, c.MethodRef);

            return a;
        }
    }
}