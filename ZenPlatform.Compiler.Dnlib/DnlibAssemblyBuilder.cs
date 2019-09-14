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
        private readonly DnlibTypeSystem _ts;
        private readonly AssemblyDefUser _assembly;
        private readonly List<ITypeBuilder> _definedTypes;

        public DnlibAssemblyBuilder(DnlibTypeSystem ts, AssemblyDefUser assembly) : base(ts, assembly)
        {
            _ts = ts;
            _assembly = assembly;
        }

        public IReadOnlyList<ITypeBuilder> DefinedTypes => _definedTypes;

        public ITypeBuilder DefineType(string @namespace, string name, TypeAttributes typeAttributes, IType baseType)
        {
            var bType = _assembly.ManifestModule.Import(((DnlibType) baseType).TypeDef);

            var type = new TypeDefUser(@namespace, name, bType);

            _assembly.ManifestModule.Types.Add(type);

            return new DnlibTypeBuilder(_ts, type, this);
        }

        public IAssembly EndBuild()
        {
            return this;
        }
    }
}