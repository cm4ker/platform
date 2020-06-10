using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Aquila.Compiler.Contracts;
using dnlib.DotNet;
using IAssembly = Aquila.Compiler.Contracts.IAssembly;
using ICustomAttribute = Aquila.Compiler.Contracts.ICustomAttribute;
using IType = Aquila.Compiler.Contracts.IType;
using TypeAttributes = System.Reflection.TypeAttributes;

namespace Aquila.Compiler.Roslyn.RoslynBackend
{
    public class RoslynAssemblyBuilder : RoslynAssembly, IAssemblyBuilder
    {
        private readonly RoslynTypeSystem _ts;
        private readonly AssemblyDefUser _assembly;
        private readonly List<RoslynTypeBuilder> _definedTypes;

        public RoslynAssemblyBuilder(RoslynTypeSystem ts, AssemblyDefUser assembly) : base(ts, assembly)
        {
            _ts = ts;
            _assembly = assembly;
            _definedTypes = new List<RoslynTypeBuilder>();
        }

        public IReadOnlyList<ITypeBuilder> DefinedTypes => _definedTypes;

        public ITypeBuilder DefineType(string @namespace, string name, TypeAttributes typeAttributes,
            IType baseType)
        {
            ITypeDefOrRef bType = ((RoslynType) baseType).TypeRef;

            if (bType is TypeRef || bType is TypeDef && bType.Module != this._assembly.ManifestModule)
                bType = (ITypeDefOrRef) _assembly.ManifestModule.Import(bType);

            var type = new TypeDefUser(@namespace, name, bType);

            type.Attributes = RoslynMapper.Convert(typeAttributes);

            _assembly.ManifestModule.Types.Add(type);

            var dnlibType = new RoslynTypeBuilder(_ts, type, this);
            _definedTypes.Add(dnlibType);
            TypeCache.Add(dnlibType.FullName, dnlibType);
            _ts.RegisterType(dnlibType);

            return dnlibType;
        }


        public void Write(string fileName)
        {
            var sb = new StringBuilder();
            Dump(new StringWriter(sb));

            RoslynCompilationHelper.GenerateAssembly(sb.ToString(), fileName, _ts.Paths.ToArray());
        }

        public void Write(Stream stream)
        {
            var tmpFile = Path.GetTempFileName();

            Write(tmpFile);

            using (FileStream fs = new FileStream(tmpFile, FileMode.OpenOrCreate))
            {
                fs.CopyTo(stream);
            }

            File.Delete(tmpFile);
        }

        public void Dump(TextWriter textWriter)
        {
            foreach (var module in _definedTypes)
            {
                module.Dump(textWriter);
            }
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