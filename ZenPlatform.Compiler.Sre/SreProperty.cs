using System.Collections.Generic;
using System;
using System.Reflection;
using System.Reflection.Emit;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Sre
{
    class SreProperty : SreMemberInfo, IProperty
    {
        private IMethod _getMethod;
        private IMethod _setMethod;
        public PropertyInfo Member { get; }

        public SreProperty(SreTypeSystem system, PropertyInfo member) : base(system, member)
        {
            Member = member;
        }

        public bool Equals(IProperty other)
        {
            var otherProp = ((SreProperty)other)?.Member;
            if (otherProp == null)
                return false;
            return otherProp?.DeclaringType?.Equals(Member.DeclaringType) == true
                   && Member.Name == otherProp.Name;
        }

        public IType PropertyType => System.ResolveType(Member.PropertyType);

        public IMethod Getter => _getMethod ??= new SreMethod(System, Member.GetMethod);

        public IMethod Setter => _setMethod ??= new SreMethod(System, Member.SetMethod);
    }

    class SrePropertyBuilder : SreMemberInfo, IPropertyBuilder
    {
        private IMethod _setter;
        private IMethod _getter;

        public PropertyBuilder Member { get; }

        public SrePropertyBuilder(SreTypeSystem system, PropertyBuilder member) : base(system, member)
        {
            Member = member;
        }

        public bool Equals(IProperty other)
        {
            var otherProp = ((SreProperty)other)?.Member;
            if (otherProp == null)
                return false;
            return otherProp?.DeclaringType?.Equals(Member.DeclaringType) == true
                   && Member.Name == otherProp.Name;
        }

        public IType PropertyType => System.ResolveType(Member.PropertyType);

        public IMethod Setter
        {
            get => _setter;
            private set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _setter = value;

                //Bake the method before we are set it to the member
                ((SreMethodBuilder)_getter).Bake();

                Member.SetSetMethod((MethodBuilder)((SreMethodBuilder)_setter).Method);
            }
        }

        public IMethod Getter
        {
            get { return _getter; }
            private set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _getter = value;

                //Bake the method before we are set it to the member
                ((SreMethodBuilder)_getter).Bake();

                Member.SetGetMethod((MethodBuilder)((SreMethodBuilder)_getter).Method);
            }
        }

        public IPropertyBuilder WithSetter(IMethod method)
        {
            Setter = method;
            return this;
        }

        public IPropertyBuilder WithGetter(IMethod method)
        {
            Getter = method;
            return this;
        }
    }
}