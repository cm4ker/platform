using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using ZenPlatform.Compiler.Contracts;
using ICustomAttribute = ZenPlatform.Compiler.Contracts.ICustomAttribute;
using TypeAttributes = System.Reflection.TypeAttributes;


namespace ZenPlatform.Compiler.Cecil
{
    public class CecilAssembly : IAssembly, IAssemblyBuilder
    {
        private Dictionary<string, CecilType> _typeCache = new Dictionary<string, CecilType>();

        public ITypeSystem TypeSystem => _typeSystem;

        public AssemblyDefinition Assembly { get; }

        public CecilAssembly(CecilTypeSystem typeSystem, AssemblyDefinition assembly)
        {
            _typeSystem = typeSystem;
            Assembly = assembly;
        }

        public bool Equals(IAssembly other) => other == this;

        public string Name => Assembly.Name.Name;
        private IReadOnlyList<ICustomAttribute> _attributes;
        private readonly CecilTypeSystem _typeSystem;

        public IReadOnlyList<ICustomAttribute> CustomAttributes =>
            _attributes ??= Assembly.CustomAttributes.Select(ca => new CecilCustomAttribute(_typeSystem, ca))
                .ToList();

        public IType FindType(string fullName)
        {
            if (_typeCache.TryGetValue(fullName, out var rv))
                return rv;
            var lastDot = fullName.LastIndexOf(".", StringComparison.Ordinal);
            var asmRef = new AssemblyNameReference(Assembly.Name.Name, Assembly.Name.Version);
            var tref = (lastDot == -1)
                ? new TypeReference(null, fullName, Assembly.MainModule, asmRef)
                : new TypeReference(fullName.Substring(0, lastDot),
                    fullName.Substring(lastDot + 1), Assembly.MainModule, asmRef);
            var resolved = tref.Resolve();
            if (resolved != null)
                return _typeCache[fullName] = _typeSystem.GetTypeFor(resolved);

            return null;
        }

        public void Write(string fileName)
        {
            Assembly.Write(fileName);
        }

        public ITypeBuilder DefineType(string @namespace, string name, TypeAttributes typeAttributes, IType baseType)
        {
            var typeDefinition = new TypeDefinition(@namespace, name, SreMapper.Convert(typeAttributes),
                _typeSystem.GetTypeReference(baseType));

            Assembly.MainModule.Types.Add(typeDefinition);

            return new CecilTypeBuilder(_typeSystem, this, typeDefinition);
        }
    }
}