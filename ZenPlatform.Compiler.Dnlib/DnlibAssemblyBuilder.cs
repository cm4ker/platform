using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.MD;
using ZenPlatform.Compiler.Contracts;
using IAssembly = ZenPlatform.Compiler.Contracts.IAssembly;
using ICustomAttribute = ZenPlatform.Compiler.Contracts.ICustomAttribute;
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
            _definedTypes = new List<ITypeBuilder>();
        }

        public IReadOnlyList<ITypeBuilder> DefinedTypes => _definedTypes;

        public ITypeBuilder DefineType(string @namespace, string name, TypeAttributes typeAttributes, IType baseType)
        {
            ITypeDefOrRef bType = ((DnlibType) baseType).TypeRef;

            if (bType is TypeRef || bType is TypeDef && bType.Module != this._assembly.ManifestModule)
                bType = (ITypeDefOrRef) _assembly.ManifestModule.Import(bType);

            var type = new TypeDefUser(@namespace, name, bType);

            type.Attributes = SreMapper.Convert(typeAttributes);

            _assembly.ManifestModule.Types.Add(type);

            var dnlibType = new DnlibTypeBuilder(_ts, type, this);
            _definedTypes.Add(dnlibType);
            TypeCache.Add(dnlibType.FullName, dnlibType);
            _ts.RegisterType(dnlibType);
            
            return dnlibType;
        }

        public ITypeBuilder ImportWithCopy(IType type)
        {
            throw new NotImplementedException();
        }

        public void SetAttribute(ICustomAttribute attr)
        {
            throw new NotImplementedException();
        }

        public IAssembly EndBuild()
        {
            return this;
        }
    }
}