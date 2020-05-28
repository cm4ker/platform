using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aquila.Compiler.Contracts;
using dnlib.DotNet;
using IAssembly = dnlib.DotNet.IAssembly;
using ICustomAttribute = Aquila.Compiler.Contracts.ICustomAttribute;
using IType = Aquila.Compiler.Contracts.IType;

namespace Aquila.Compiler.Roslyn.RoslynBackend
{
    public class RoslynAssembly : Contracts.IAssembly
    {
        private RoslynTypeSystem _ts;

        protected Dictionary<string, RoslynType> TypeCache = new Dictionary<string, RoslynType>();

        public RoslynAssembly(RoslynTypeSystem ts, AssemblyDef assembly)
        {
            if (assembly is null)
                throw new NullReferenceException("Assembly");

            Assembly = assembly;
            _ts = ts;

            if (string.IsNullOrEmpty(Assembly.Name))
                throw new NullReferenceException("Name");
        }

        public AssemblyDef Assembly { get; }


        public bool Equals(IAssembly other) => other == this;

        public string Name => Assembly.Name;

        private IReadOnlyList<RoslynCustomAttribute> _attributes;

        public IReadOnlyList<ICustomAttribute> CustomAttributes =>
            _attributes ??= Assembly.CustomAttributes.Select(ca => new RoslynCustomAttribute(_ts, ca))
                .ToList();

        public IType FindType(string fullName)
        {
            if (TypeCache.TryGetValue(fullName, out var rv))
                return rv;
            var lastDot = fullName.LastIndexOf(".", StringComparison.Ordinal);

            if (!_ts.AsmRefsCache.TryGetValue(Assembly.FullName, out var asmRef))
            {
                Console.Write(Assembly.FullName);

                try
                {
                    asmRef = new AssemblyNameInfo(Assembly.FullName).ToAssemblyRef();
                }
                catch (Exception ex)
                {
                }

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
            throw new NotImplementedException();
        }

        public void Write(Stream stream)
        {
            throw new NotImplementedException();
        }

        public ITypeSystem TypeSystem => _ts;

        public bool Equals(Contracts.IAssembly other)
        {
            throw new NotImplementedException();
        }
    }
}