using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Sre
{
    class SreAssemblyBuilder : IAssemblyBuilder
    {
        private readonly SreTypeSystem _system;
        private IReadOnlyList<ICustomAttribute> _customAttributes;
        public AssemblyBuilder Assembly { get; }
        public ModuleBuilder MainModule { get; }
        public void Write(Stream stream)
        {
            throw new NotImplementedException();
        }

        public ITypeSystem TypeSystem => _system;

        private List<ITypeBuilder> _definedTypes = new List<ITypeBuilder>();


        public SreAssemblyBuilder(SreTypeSystem system, AssemblyBuilder asm)
        {
            _system = system;
            Assembly = asm;
            Assembly.GetName().Version = new Version(1, 0);
            MainModule = Assembly.DefineDynamicModule("SomeModule");
        }

        public bool Equals(IAssembly other) => Assembly == ((SreAssembly) other)?.Assembly;

        public string Name => Assembly.GetName().Name;

        public IReadOnlyList<IType> Types { get; private set; }


        private Dictionary<string, SreType> _typeDic = new Dictionary<string, SreType>();

        public IReadOnlyList<ICustomAttribute> CustomAttributes
            => _customAttributes ??= Assembly.GetCustomAttributesData().Select(a => new SreCustomAttribute(
                _system, a, _system.ResolveType(a.AttributeType))).ToList();

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

        public IReadOnlyList<ITypeBuilder> DefinedTypes => _definedTypes;

        public ITypeBuilder DefineType(string @namespace, string name, TypeAttributes typeAttributes, IType baseType)
        {
            var typeName = $"{@namespace}.{name}";
            var type = new SreTypeBuilder(_system,
                MainModule.DefineType(typeName, typeAttributes, _system.GetType(baseType)));

            _typeDic.Add(typeName, type);
            _definedTypes.Add(type);

            return type;
        }

        public IAssembly EndBuild()
        {
            foreach (var defType in _definedTypes)
            {
                defType.EndBuild();
            }

            return this;
        }
    }
}