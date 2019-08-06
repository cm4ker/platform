using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.MD;
using ZenPlatform.Compiler.Contracts;
using IAssembly = ZenPlatform.Compiler.Contracts.IAssembly;
using IType = ZenPlatform.Compiler.Contracts.IType;
using TypeAttributes = System.Reflection.TypeAttributes;

namespace ZenPlatform.Compiler.Dnlib
{
    public class DnlibAssemblyBuilder : DnlibAssembly, IAssemblyBuilder
    {
        private readonly AssemblyDefUser _assembly;
        private readonly List<ITypeBuilder> _definedTypes;

        public DnlibAssemblyBuilder(ITypeSystem ts, AssemblyDefUser assembly) : base(ts, assembly)
        {
            _assembly = assembly;
        }

        public IReadOnlyList<ITypeBuilder> DefinedTypes => _definedTypes;

        public ITypeBuilder DefineType(string @namespace, string name, TypeAttributes typeAttributes, IType baseType)
        {
            var dnlibBaseType = (DnlibType) baseType;

            var type = new TypeDefUser(@namespace, name, dnlibBaseType.TypeDef);

            _assembly.ManifestModule.Types.Add(type);

            return new DnlibTypeBuilder(type, this);
        }

        public IAssembly EndBuild()
        {
            return this;
        }
    }
}