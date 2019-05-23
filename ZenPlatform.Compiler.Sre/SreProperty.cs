using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Sre
{
    class SreProperty : SreMemberInfo, IProperty
    {
        public PropertyInfo Member { get; }

        public SreProperty(SreTypeSystem system, PropertyInfo member) : base(system, member)
        {
            Member = member;
            Setter = member.SetMethod == null ? null : new SreMethod(system, member.SetMethod);
            Getter = member.GetMethod == null ? null : new SreMethod(system, member.GetMethod);
        }

        public bool Equals(IProperty other)
        {
            var otherProp = ((SreProperty) other)?.Member;
            if (otherProp == null)
                return false;
            return otherProp?.DeclaringType?.Equals(Member.DeclaringType) == true
                   && Member.Name == otherProp.Name;
        }

        public IType PropertyType => System.ResolveType(Member.PropertyType);
        public IMethod Setter { get; }
        public IMethod Getter { get; }
    }

    class SrePropertyBuilder : SreMemberInfo, IPropertyBuilder
    {
        public PropertyInfo Member { get; }

        public SrePropertyBuilder(SreTypeSystem system, PropertyInfo member) : base(system, member)
        {
            Member = member;
            Setter = member.SetMethod == null ? null : new SreMethod(system, member.SetMethod);
            Getter = member.GetMethod == null ? null : new SreMethod(system, member.GetMethod);
        }

        public bool Equals(IProperty other)
        {
            var otherProp = ((SreProperty) other)?.Member;
            if (otherProp == null)
                return false;
            return otherProp?.DeclaringType?.Equals(Member.DeclaringType) == true
                   && Member.Name == otherProp.Name;
        }

        public IType PropertyType => System.ResolveType(Member.PropertyType);
        public IMethod Setter { get; }
        public IMethod Getter { get; }

        public IPropertyBuilder WithPropType(IType propertyType)
        {
            return 
        }

        public IPropertyBuilder WithSetter(IMethod method)
        {
            throw new System.NotImplementedException();
        }

        public IPropertyBuilder WithGetter(IMethod method)
        {
            throw new System.NotImplementedException();
        }
    }
}