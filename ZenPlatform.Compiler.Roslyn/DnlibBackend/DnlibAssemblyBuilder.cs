using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using dnlib.DotNet;
using TypeAttributes = System.Reflection.TypeAttributes;

namespace ZenPlatform.Compiler.Roslyn.DnlibBackend
{
    public class SreAssemblyBuilder : SreAssembly
    {
        private readonly SreTypeSystem _ts;
        private readonly AssemblyDefUser _assembly;
        private readonly List<SreTypeBuilder> _definedTypes;

        public SreAssemblyBuilder(SreTypeSystem ts, AssemblyDefUser assembly) : base(ts, assembly)
        {
            _ts = ts;
            _assembly = assembly;
            _definedTypes = new List<SreTypeBuilder>();
        }

        public IReadOnlyList<SreTypeBuilder> DefinedTypes => _definedTypes;

        public SreTypeBuilder DefineType(string @namespace, string name, TypeAttributes typeAttributes,
            SreType baseType)
        {
            ITypeDefOrRef bType = baseType.TypeRef;

            if (bType is TypeRef || bType is TypeDef && bType.Module != this._assembly.ManifestModule)
                bType = (ITypeDefOrRef) _assembly.ManifestModule.Import(bType);

            var type = new TypeDefUser(@namespace, name, bType);

            type.Attributes = SreMapper.Convert(typeAttributes);

            _assembly.ManifestModule.Types.Add(type);

            var dnlibType = new SreTypeBuilder(_ts, type, this);
            _definedTypes.Add(dnlibType);
            TypeCache.Add(dnlibType.FullName, dnlibType);
            _ts.RegisterType(dnlibType);

            return dnlibType;
        }


        public void Write(string fileName)
        {
            var sb = new StringBuilder();
            Dump(new StringWriter(sb));

            Console.Write(sb);

            EmitDemo.GenerateAssembly(sb.ToString(), fileName, _ts.Paths.ToArray());
        }

        public void Write(Stream stream)
        {
            throw new NotImplementedException();
        }

        public void Dump(TextWriter textWriter)
        {
            foreach (var module in _definedTypes)
            {
                module.Dump(textWriter);
            }
        }


        public SreTypeBuilder ImportWithCopy(SreType type)
        {
            throw new NotImplementedException();
        }

        public void SetAttribute(ICustomAttribute attr)
        {
            throw new NotImplementedException();
        }

        public SreAssembly EndBuild()
        {
            return this;
        }
    }
}