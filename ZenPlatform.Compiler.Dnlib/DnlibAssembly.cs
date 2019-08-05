using System;
using System.Collections.Generic;
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
        public DnlibAssembly(ITypeSystem ts, AssemblyDef assembly)
        {
            Assembly = assembly;
            TypeSystem = ts;
            
        }

        public AssemblyDef Assembly { get; }


        public bool Equals(IAssembly other) => other == this;

        public string Name => Assembly.Name;

        private IReadOnlyList<ICustomAttribute> _attributes;
        private readonly DnlibTypeSystem _typeSystem;

        public IReadOnlyList<ICustomAttribute> CustomAttributes =>
            _attributes ??= Assembly.CustomAttributes.Select(ca => new DnlibCusotmAtttribute(_typeSystem, ca))
                .ToList();

        public IType FindType(string fullName)
        {
            throw new NotImplementedException();
        }

        public void Write(string fileName)
        {
            Assembly.Write(fileName);
        }

        public ITypeSystem TypeSystem { get; }
    }
}