using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Aquila.Compiler.Contracts;

namespace Aquila.Compiler.Sre
{
    class SreMemberInfo
    {
        protected readonly SreTypeSystem System;
        private readonly MemberInfo _member;
        private IReadOnlyList<ICustomAttribute> _customAttributes;
        public string Name => _member.Name;

        public SreMemberInfo(SreTypeSystem system, MemberInfo member)
        {
            System = system;
            _member = member;
        }

        public IReadOnlyList<ICustomAttribute> CustomAttributes
            => _customAttributes ??
               (_customAttributes = _member.GetCustomAttributesData().Select(a => new SreCustomAttribute(System,
                   a, System.ResolveType(a.AttributeType))).ToList());
    }
}