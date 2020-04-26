using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Sre
{
    class SreAssembly : IAssembly
    {
        private readonly SreTypeSystem _system;
        private IReadOnlyList<ICustomAttribute> _customAttributes;
        public Assembly Assembly { get; }
        public ITypeSystem TypeSystem => _system;

        public SreAssembly(SreTypeSystem system, Assembly asm)
        {
            _system = system;
            Assembly = asm;
        }

        public bool Equals(IAssembly other) => Assembly == ((SreAssembly) other)?.Assembly;

        public string Name => Assembly.GetName().Name;

        public IReadOnlyList<IType> Types { get; private set; }
        private Dictionary<string, SreType> _typeDic = new Dictionary<string, SreType>();

        public IReadOnlyList<ICustomAttribute> CustomAttributes
            => _customAttributes ??
               (_customAttributes = Assembly.GetCustomAttributesData().Select(a => new SreCustomAttribute(
                   _system, a, _system.ResolveType(a.AttributeType))).ToList());

        public IType FindType(string fullName)
        {
            _typeDic.TryGetValue(fullName, out var rv);
            return rv;
        }

        public void Write(string fileName)
        {
            throw new NotImplementedException();
//            var generator = new AssemblyGenerator();
//            generator.GenerateAssembly(Assembly, fileName);
        }

        public void Write(Stream stream)
        {
            throw new NotImplementedException();
//            var generator = new AssemblyGenerator();
//            stream.Write(generator.GenerateAssemblyBytes(Assembly));
        }

        public void Init()
        {
            var types = Assembly.GetExportedTypes().Select(t => _system.ResolveType(t)).ToList();
            Types = types;
            _typeDic = types.ToDictionary(t => t.Type.FullName);
        }
    }
}