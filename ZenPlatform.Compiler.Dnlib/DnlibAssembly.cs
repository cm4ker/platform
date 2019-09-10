using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dnlib.DotNet;
using ZenPlatform.Compiler.Contracts;
using IAssembly = ZenPlatform.Compiler.Contracts.IAssembly;
using ICustomAttribute = ZenPlatform.Compiler.Contracts.ICustomAttribute;
using IType = ZenPlatform.Compiler.Contracts.IType;

namespace ZenPlatform.Compiler.Dnlib
{
    public class DnlibAssembly : IAssembly
    {
        private Dictionary<string, DnlibType> _typeCache = new Dictionary<string, DnlibType>();

        public DnlibAssembly(DnlibTypeSystem ts, AssemblyDef assembly)
        {
            Assembly = assembly;
            TypeSystem = ts;
        }

        public AssemblyDef Assembly { get; }


        public bool Equals(IAssembly other) => other == this;

        public string Name => Assembly.Name;

        private IReadOnlyList<ICustomAttribute> _attributes;
        private readonly DnlibTypeSystem _ts;

        public IReadOnlyList<ICustomAttribute> CustomAttributes =>
            _attributes ??= Assembly.CustomAttributes.Select(ca => new DnlibCusotmAtttribute(_ts, ca))
                .ToList();

        public IType FindType(string fullName)
        {
            if (_typeCache.TryGetValue(fullName, out var rv))
                return rv;
            var lastDot = fullName.LastIndexOf(".", StringComparison.Ordinal);
            var asmRef = new AssemblyNameInfo(Assembly.FullName);

            var resolved = Assembly.Find(fullName, false).ResolveTypeDef();
            if (resolved != null)
                return _typeCache[fullName] = _ts.GetTypeFor(resolved);

            return null;
        }

        public void Write(string fileName)
        {
            Assembly.Write(fileName);
        }

        public void Write(Stream stream)
        {
            Assembly.Write(stream);
        }

        public ITypeSystem TypeSystem { get; }
    }
}