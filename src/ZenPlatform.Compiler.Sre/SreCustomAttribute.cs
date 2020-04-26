using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Sre
{
    class SreCustomAttribute : ICustomAttribute
    {
        private readonly CustomAttributeData _data;

        public SreCustomAttribute(SreTypeSystem system, CustomAttributeData data, IType type)
        {
            Type = type;
            _data = data;
            Parameters = data.ConstructorArguments.Select(p =>
                p.Value is Type t ? system.ResolveType(t) : p.Value
            ).ToList();
            Properties = data.NamedArguments?.ToDictionary(x => x.MemberName, x => x.TypedValue.Value) ??
                         new Dictionary<string, object>();
        }

        public bool Equals(ICustomAttribute other)
        {
            return ((SreCustomAttribute) other)?._data.Equals(_data) == true;
        }

        public IType Type { get; }
        public List<object> Parameters { get; }
        public Dictionary<string, object> Properties { get; }
    }
}