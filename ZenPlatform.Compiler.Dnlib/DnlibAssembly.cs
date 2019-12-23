using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.MD;
using ZenPlatform.Compiler.Contracts;
using IAssembly = ZenPlatform.Compiler.Contracts.IAssembly;
using ICustomAttribute = ZenPlatform.Compiler.Contracts.ICustomAttribute;
using IType = ZenPlatform.Compiler.Contracts.IType;

namespace ZenPlatform.Compiler.Dnlib
{
    public class DnlibAssembly : IAssembly
    {
        private DnlibTypeSystem _ts;

        protected Dictionary<string, DnlibType> _typeCache = new Dictionary<string, DnlibType>();

        public DnlibAssembly(DnlibTypeSystem ts, AssemblyDef assembly)
        {
            Assembly = assembly;
            _ts = ts;
        }

        public AssemblyDef Assembly { get; }


        public bool Equals(IAssembly other) => other == this;

        public string Name => Assembly.Name;

        private IReadOnlyList<ICustomAttribute> _attributes;

        public IReadOnlyList<ICustomAttribute> CustomAttributes =>
            _attributes ??= Assembly.CustomAttributes.Select(ca => new DnlibCusotmAtttribute(_ts, ca))
                .ToList();

      

        public IType FindType(string fullName)
        {
            if (_typeCache.TryGetValue(fullName, out var rv))
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
                return _typeCache[fullName] = _ts.GetTypeFromReference(tref);

            return null;
        }

        public void Write(string fileName)
        {
            //var asm = Assembly.ManifestModule.ResolveToken(new MDToken(Table.AssemblyRef, 1).Raw);
        Assembly.Write(fileName);
        }

        public void Write(Stream stream)
        {
            //var asm = Assembly.ManifestModule.ResolveToken(new MDToken(Table.AssemblyRef, 1).Raw);
            Assembly.Write(stream);
        }

        public ITypeSystem TypeSystem => _ts;
    }
}