using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;

namespace ZenPlatform.Compiler.Roslyn.DnlibBackend
{
    public class SreAssembly
    {
        private SreTypeSystem _ts;

        protected Dictionary<string, SreType> TypeCache = new Dictionary<string, SreType>();

        public SreAssembly(SreTypeSystem ts, AssemblyDef assembly)
        {
            Assembly = assembly;
            _ts = ts;
        }

        public AssemblyDef Assembly { get; }


        public bool Equals(IAssembly other) => other == this;

        public string Name => Assembly.Name;

        private IReadOnlyList<SreCustomAttribute> _attributes;

        public IReadOnlyList<SreCustomAttribute> CustomAttributes =>
            _attributes ??= Assembly.CustomAttributes.Select(ca => new SreCustomAttribute(_ts, ca))
                .ToList();


        public SreType FindType(string fullName)
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

        public SreTypeSystem TypeSystem => _ts;
    }
}