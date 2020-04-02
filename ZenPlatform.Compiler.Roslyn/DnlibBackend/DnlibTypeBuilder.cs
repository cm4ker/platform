using System;
using System.Collections.Generic;
using System.IO;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace ZenPlatform.Compiler.Roslyn.DnlibBackend
{
    public class SreTypeBuilder : SreType
    {
        private readonly SreTypeSystem _ts;
        private SreContextResolver _r;

        private List<SrePropertyBuilder> _properties = new List<SrePropertyBuilder>();
        private List<SreFieldBuilder> _fields = new List<SreFieldBuilder>();
        private List<SreMethodBuilder> _methods = new List<SreMethodBuilder>();
        private List<SreConstructorBuilder> _constructors = new List<SreConstructorBuilder>();
        private List<SreType> _interfaces = new List<SreType>();
        private List<SreType> _genericArguments = new List<SreType>();
        private List<ICustomAttribute> _customAttributes = new List<ICustomAttribute>();


        public SreTypeBuilder(SreTypeSystem typeSystem, TypeDef typeDef, SreAssembly assembly)
            : base(typeSystem, typeDef, typeDef, assembly)
        {
            _ts = typeSystem;
            _r = new SreContextResolver(_ts, typeDef.Module);
        }


        public override IReadOnlyList<SreField> Fields => _fields;

        public override IReadOnlyList<SreConstructor> Constructors => _constructors;

        public override IReadOnlyList<SreMethod> Methods => _methods;

        public override IReadOnlyList<SreType> Interfaces => _interfaces;

        public override IReadOnlyList<ICustomAttribute> CustomAttributes => _customAttributes;

        public override IReadOnlyList<SreProperty> Properties => _properties;


        public void AddInterfaceImplementation(SreType type)
        {
            var dType = (SreType) type;
            this.TypeDef.Interfaces.Add(new InterfaceImplUser(dType.TypeRef));
        }

        public void DefineGenericParameters(IReadOnlyList<KeyValuePair<string, GenericParameterConstraint>> names)
        {
            throw new NotImplementedException();
        }

        public SreField DefineField(SreType type, string name, bool isPublic, bool isStatic)
        {
            var tref = _r.GetReference(((SreType) type).TypeRef);
            var field = new FieldDefUser(name, new FieldSig(tref.ToTypeSig()));
            field.IsStatic = isStatic;
            field.Access |= (isPublic) ? FieldAttributes.Public : FieldAttributes.Private;
            field.DeclaringType = TypeDef;

            var dfield = new SreFieldBuilder(_ts, field);

            _fields.Add(dfield);

            return dfield;
        }

        public SreMethodBuilder DefineMethod(string name, bool isPublic, bool isStatic, bool isInterfaceImpl,
            SreMethod overrideMethod = null, bool isVirtual = false)
        {
            MethodSig sig;

            if (isStatic)
                sig = MethodSig.CreateStatic(_ts.GetSystemBindings().Void.GetRef().ToTypeSig());
            else
                sig = MethodSig.CreateInstance(_ts.GetSystemBindings().Void.GetRef().ToTypeSig());

            var method = new MethodDefUser(name, sig);

            var dm = new SreMethodBuilder(_ts, method, TypeRef);
            _methods.Add(dm);


            method.Attributes |= (isPublic) ? MethodAttributes.Public : MethodAttributes.Private;

            method.IsStatic = isStatic;

            if (overrideMethod != null)
            {
                var mo = new MethodOverride(method, (IMethodDefOrRef) overrideMethod.MethodRef);
                method.Overrides.Add(mo);
                method.Attributes |= MethodAttributes.Virtual;
            }

            if (isInterfaceImpl)
            {
                method.Attributes |= MethodAttributes.NewSlot | MethodAttributes.Virtual;
            }

            if (isVirtual)
                method.Attributes |= MethodAttributes.Virtual;

            method.Attributes |= MethodAttributes.HideBySig;

            method.DeclaringType = TypeDef;
            method.Body = new CilBody();

            method.Body.InitLocals = true;
            //method.MethodSig = new MethodSig(c);

            method.ReturnType = _r.GetReference(_ts.GetSystemBindings().Void.ToTypeRef()).ToTypeSig();

            return dm;
        }

        public SrePropertyBuilder DefineProperty(SreType propertyType, string name, bool isStatic = false)
        {
            var prop = new PropertyDefUser(name);

            TypeDef.Properties.Add(prop);
            prop.DeclaringType = TypeDef;

            if (isStatic)
                prop.PropertySig = PropertySig.CreateStatic(_r.GetReference(propertyType.GetRef()).ToTypeSig(),
                    _r.GetReference(propertyType.GetRef()).ToTypeSig());
            else
                prop.PropertySig = PropertySig.CreateInstance(_r.GetReference(propertyType.GetRef()).ToTypeSig(),
                    _r.GetReference(propertyType.GetRef()).ToTypeSig());

            var propertyBuilder = new SrePropertyBuilder(_ts, prop, TypeRef);
            _properties.Add(propertyBuilder);

            return propertyBuilder;
        }

        public SreConstructorBuilder DefineConstructor(bool isStatic, params SreType[] args)
        {
            MethodSig sig;

            if (isStatic)
                sig = MethodSig.CreateStatic(_ts.GetSystemBindings().Void.GetRef().ToTypeSig());
            else
                sig = MethodSig.CreateInstance(_ts.GetSystemBindings().Void.GetRef().ToTypeSig());


            var name = (isStatic) ? ".cctor" : ".ctor";
            var c = new MethodDefUser(name, sig);

            c.IsStatic = isStatic;

            c.Attributes |= MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.Public |
                            MethodAttributes.HideBySig;

            c.DeclaringType = TypeDef;

            var dc = new SreConstructorBuilder(_ts, c, TypeRef);


            foreach (var arg in args)
            {
                dc.DefineParameter(arg);
                //  sig.Params.Add(arg.GetRef().ToTypeSig());
            }

            _constructors.Add(dc);

            return dc;
        }

        public SreTypeBuilder DefineNastedType(SreType baseType, string name, bool isPublic)
        {
            throw new NotImplementedException();
        }

        public void SetCustomAttribute(ICustomAttribute attr)
        {
            var dnlibAttr = (SreCustomAttribute) attr;
            dnlibAttr.ImportAttribute(TypeDef.Module);
            ((List<SreCustomAttribute>) CustomAttributes).Add(dnlibAttr);
            TypeDef.CustomAttributes.Add(dnlibAttr.GetCA());
        }

        public void Dump(TextWriter sw)
        {
            Pair cb = null;
            if (!string.IsNullOrEmpty(Namespace))
            {
                sw.Write("\nnamespace ");
                sw.Write(Namespace);
                cb = sw.CurlyBrace();
            }

            if (IsPublic)
                sw.Write("public ");

            if (IsAbstract)
                sw.Write("abstract ");

            sw.Write("class ");
            sw.Write(Name);

            sw.W(":");
            BaseType.DumpRef(sw);

            using (sw.CurlyBrace())
            {
                foreach (var field in _fields)
                {
                    field.Dump(sw);
                    sw.W("\n");
                }

                foreach (var constructor in _constructors)
                {
                    constructor.Dump(sw);
                }

                foreach (var prop in _properties)
                {
                    prop.Dump(sw);
                    sw.W("\n");
                }

                foreach (var method in _methods)
                {
                    method.Dump(sw);
                }

                //Internal code here
            }

            if (!string.IsNullOrEmpty(Namespace))
            {
                cb.Dispose();
            }

            sw.WriteLine();
        }
    }
}