using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Lokad.ILPack;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Sre
{
    class SreAssembly : IAssembly
    {
        private readonly SreTypeSystem _system;
        private IReadOnlyList<ICustomAttribute> _customAttributes;
        public Assembly Assembly { get; }

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
            var generator = new AssemblyGenerator();
            generator.GenerateAssembly(Assembly, fileName);
        }

        public void Init()
        {
            var types = Assembly.GetExportedTypes().Select(t => _system.ResolveType(t)).ToList();
            Types = types;
            _typeDic = types.ToDictionary(t => t.Type.FullName);
        }
    }

    class SreDynamicAssembly : IAssembly
    {
        private readonly SreTypeSystem _system;
        private IReadOnlyList<ICustomAttribute> _customAttributes;
        public AssemblyBuilder Assembly { get; }

        public SreDynamicAssembly(SreTypeSystem system, AssemblyBuilder asm)
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
            var generator = new AssemblyGenerator();
            generator.GenerateAssembly(Assembly, fileName);
        }

        public void Init()
        {
            var types = Assembly.GetExportedTypes().Select(t => _system.ResolveType(t)).ToList();
            Types = types;
            _typeDic = types.ToDictionary(t => t.Type.FullName);
        }
    }


    public class SreAssemblyFactory : IAssemblyFactory
    {
        public IAssembly Create(ITypeSystem ts, string assemblyName, Version assemblyVersion)
        {
            var da = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString("N")),
                AssemblyBuilderAccess.Run);

            return new SreDynamicAssembly((SreTypeSystem) ts, da);
        }
    }
}