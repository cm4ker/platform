using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Sre
{
    class SreTypeBuilder : SreType, ITypeBuilder
    {
        private readonly SreTypeSystem _system;
        private readonly TypeBuilder _tb;
        private readonly List<SreMethodBuilder> _definedMethods = new List<SreMethodBuilder>();

        public SreTypeBuilder(SreTypeSystem system, TypeBuilder tb) : base(system, null, tb)
        {
            _system = system;
            _tb = tb;
        }

        public IField DefineField(IType type, string name, bool isPublic, bool isStatic)
        {
            var f = _tb.DefineField(name, ((SreType)type).Type,
                (isPublic ? FieldAttributes.Public : FieldAttributes.Private)
                | (isStatic ? FieldAttributes.Static : default(FieldAttributes)));
            return new SreField(_system, f);
        }

        public void AddInterfaceImplementation(IType type)
        {
            _tb.AddInterfaceImplementation(((SreType)type).Type);
        }


        public IMethodBuilder DefineMethod(string name,
            bool isPublic, bool isStatic,
            bool isInterfaceImpl, IMethod overrideMethod)
        {
            var m = _tb.DefineMethod(name,
                (isPublic ? MethodAttributes.Public : MethodAttributes.Private)
                | (isStatic ? MethodAttributes.Static : default(MethodAttributes))
                | (isInterfaceImpl ? MethodAttributes.Virtual | MethodAttributes.NewSlot : default(MethodAttributes)));

            if (overrideMethod != null)
                _tb.DefineMethodOverride(m, ((SreMethod)overrideMethod).Method);

            var result = new SreMethodBuilder(_system, m);
            _definedMethods.Add(result);
            return result;
        }

        public IPropertyBuilder DefineProperty(IType propertyType, string name)
        {
            var propBuilder = _tb.DefineProperty(name, PropertyAttributes.None, ((SreType)propertyType).Type, null);

            return new SrePropertyBuilder(this._system, propBuilder);
        }

        public IConstructorBuilder DefineConstructor(bool isStatic, params IType[] args)
        {
            var attrs = MethodAttributes.Public;
            if (isStatic)
                attrs |= MethodAttributes.Static;
            var ctor = _tb.DefineConstructor(attrs,
                CallingConventions.Standard,
                args.Cast<SreType>().Select(t => t.Type).ToArray());
            return new SreConstructorBuilder(_system, ctor);
        }

        public IType CreateType() => new SreType(_system, null, _tb.CreateTypeInfo());

        public ITypeBuilder DefineNastedType(IType baseType, string name, bool isPublic)
        {
            var attrs = TypeAttributes.Class;
            if (isPublic)
                attrs |= TypeAttributes.NestedPublic;
            else
                attrs |= TypeAttributes.NestedPrivate;

            var builder = _tb.DefineNestedType(name, attrs,
                ((SreType)baseType).Type);

            return new SreTypeBuilder(_system, builder);
        }

        public IType EndBuild()
        {
            foreach (SreMethodBuilder method in _definedMethods)
            {
                method.Bake();
            }

            var resType = _tb.CreateType();
            return new SreType(_system, new SreAssembly(_system, resType.Assembly), resType);
        }

        public void DefineGenericParameters(IReadOnlyList<KeyValuePair<string, GenericParameterConstraint>> args)
        {
            var builders = _tb.DefineGenericParameters(args.Select(x => x.Key).ToArray());
            for (var c = 0; c < args.Count; c++)
            {
                if (args[c].Value.IsClass)
                    builders[c].SetGenericParameterAttributes(GenericParameterAttributes.ReferenceTypeConstraint);
            }
        }
    }


    class SreConstructorBuilder : SreConstructor, IConstructorBuilder
    {
        public SreConstructorBuilder(SreTypeSystem system, ConstructorBuilder ctor) : base(system, ctor)
        {
            Generator = new SreEmitter(system, new SreConstructorEmitterProvider(ctor));
        }

        public IEmitter Generator { get; }
    }
}