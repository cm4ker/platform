using System;
using System.Collections.Generic;
using dnlib.DotNet;
using ZenPlatform.Compiler.Contracts;
using ICustomAttribute = ZenPlatform.Compiler.Contracts.ICustomAttribute;
using IType = ZenPlatform.Compiler.Contracts.IType;

namespace ZenPlatform.Compiler.Dnlib
{
    public class DnlibCusotmAtttribute : ICustomAttribute
    {
        private readonly DnlibCustomAttribute _ca;

        public DnlibCusotmAtttribute(ITypeSystem ts, DnlibCustomAttribute ca)
        {
            _ca = ca;
        }

        public bool Equals(ICustomAttribute other)
        {
            throw new NotImplementedException();
        }

        public IType Type { get; }
        public List<object> Parameters { get; }
        public Dictionary<string, object> Properties { get; }
    }
}