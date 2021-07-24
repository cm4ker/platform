using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;
using dnlib.PE;
using Aquila.Compiler.Contracts;
using IAssembly = Aquila.Compiler.Contracts.IAssembly;
using ICustomAttribute = Aquila.Compiler.Contracts.ICustomAttribute;
using IModule = Aquila.Compiler.Contracts.IModule;
using IType = Aquila.Compiler.Contracts.IType;

namespace Aquila.Compiler.Dnlib
{
    public class DnlibAssembly : IAssembly
    {
        private DnlibTypeSystem _ts;

        private List<DnlibModule> _modules;
        protected Dictionary<string, DnlibType> TypeCache = new Dictionary<string, DnlibType>();

        public DnlibAssembly(DnlibTypeSystem ts, AssemblyDef assembly)
        {
            Assembly = assembly;
            _ts = ts;

            _modules = assembly.Modules.Select(x => new DnlibModule(ts, x)).ToList();
        }

        public AssemblyDef Assembly { get; }


        public bool Equals(IAssembly other) => other == this;

        public string Name => Assembly.Name;

        public IReadOnlyList<IModule> Modules => _modules;

        private IReadOnlyList<ICustomAttribute> _attributes;

        public IReadOnlyList<ICustomAttribute> CustomAttributes =>
            _attributes ??= Assembly.CustomAttributes.Select(ca => new DnlibCustomAttribute(_ts, ca))
                .ToList();


        public IType FindType(string fullName)
        {
            if (TypeCache.TryGetValue(fullName, out var rv))
                return rv;
            var lastDot = fullName.LastIndexOf(".", StringComparison.Ordinal);

            if (!_ts.AsmRefsCache.TryGetValue(Assembly.FullName, out var asmRef))
            {
                asmRef = new AssemblyNameInfo(Assembly.FullName).ToAssemblyRef();
                asmRef.Attributes = AssemblyAttributes.None;
                _ts.AsmRefsCache.Add(Assembly.FullName, asmRef);
            }

            var tref = (lastDot == -1)
                ? new TypeRefUser(Assembly.ManifestModule, null, fullName, asmRef)
                : new TypeRefUser(Assembly.ManifestModule, fullName.Substring(0, lastDot),
                    fullName.Substring(lastDot + 1), asmRef);

            var resolved = tref.Resolve(Assembly.ManifestModule);
            if (resolved != null)
                return TypeCache[fullName] = _ts.GetTypeFromReference(tref);

            return null;
        }

        public void Write(string fileName)
        {
            //var asm = Assembly.ManifestModule.ResolveToken(new MDToken(Table.AssemblyRef, 1).Raw);
            Assembly.Write(fileName, new ModuleWriterOptions(Assembly.ManifestModule)
            {
                PEHeadersOptions = new PEHeadersOptions()
                {
                    ImageBase = 0x00400000,
                    Subsystem = Subsystem.WindowsCui,
                    MajorLinkerVersion = 8
                },
                AddCheckSum = true,
                ModuleKind = ModuleKind.Dll
            });
        }

        public void Write(Stream stream)
        {
            //var asm = Assembly.ManifestModule.ResolveToken(new MDToken(Table.AssemblyRef, 1).Raw);
            Assembly.Write(stream, new ModuleWriterOptions(Assembly.ManifestModule)
            {
                PEHeadersOptions = new PEHeadersOptions()
                {
                    ImageBase = 0x00400000,
                    Subsystem = Subsystem.WindowsCui,
                    MajorLinkerVersion = 8
                },
                ModuleKind = ModuleKind.Dll
            });
        }

        public ITypeSystem TypeSystem => _ts;
    }
}