using System;
using System.Collections.Generic;
using dnlib.DotNet;
using ZenPlatform.Compiler.Contracts;
using IAssembly = ZenPlatform.Compiler.Contracts.IAssembly;
using IType = ZenPlatform.Compiler.Contracts.IType;

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

        public IType Resolve(TypeDef type)
        {
            throw new NotImplementedException();
        }
    }
}