using System;
using System.Collections.Generic;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Dnlib
{
    public class DnlibTypeSystem : ITypeSystem
    {
        public IReadOnlyList<IAssembly> Assemblies { get; }

        public IAssembly FindAssembly(string substring)
        {
            throw new NotImplementedException();
        }

        public IType FindType(string name)
        {
            throw new NotImplementedException();
        }

        public IType FindType(string name, string assembly)
        {
            throw new NotImplementedException();
        }
    }
}