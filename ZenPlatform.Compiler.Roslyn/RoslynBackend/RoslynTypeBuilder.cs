using System;
using System.Collections.Generic;
using System.IO;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace ZenPlatform.Compiler.Roslyn.RoslynBackend
{
    public class RoslynTypeBuilder : RoslynType
    {
        private readonly RoslynTypeSystem _ts;
        private RoslynContextResolver _r;

        private List<RoslynPropertyBuilder> _properties = new List<RoslynPropertyBuilder>();
        private List<RoslynFieldBuilder> _fields = new List<RoslynFieldBuilder>();
        private List<RoslynMethodBuilder> _methods = new List<RoslynMethodBuilder>();
        private List<RoslynConstructorBuilder> _constructors = new List<RoslynConstructorBuilder>();
        private List<RoslynType> _interfaces = new List<RoslynType>();
        private List<RoslynType> _genericArguments = new List<RoslynType>();
        private List<ICustomAttribute> _customAttributes = new List<ICustomAttribute>();


        public RoslynTypeBuilder(RoslynTypeSystem typeSystem, TypeDef typeDef, RoslynAssembly assembly)
            : base(typeSystem, typeDef, typeDef, assembly)
        {
            _ts = typeSystem;
            _r = new RoslynContextResolver(_ts, typeDef.Module);
        }


        public override IReadOnlyList<RoslynField> Fields => _fields;

        public override IReadOnlyList<RoslynConstructor> Constructors => _constructors;

        public override IReadOnlyList<RoslynMethod> Methods => _methods;

        public override IReadOnlyList<RoslynType> Interfaces => _interfaces;

        public override IReadOnlyList<ICustomAttribute> CustomAttributes => _customAttributes;

        public override IReadOnlyList<RoslynProperty> Properties => _properties;


        public void AddInterfaceImplementation(RoslynType type)
        {
            var dType = (RoslynType) type;
            this.TypeDef.Interfaces.Add(new InterfaceImplUser(dType.TypeRef));
        }

        public void DefineGenericParameters(IReadOnlyList<KeyValuePair<string, GenericParameterConstraint>> names)
        {
            throw new NotImplementedException();
        }

        public RoslynField DefineField(RoslynType type, string name, bool isPublic, bool isStatic)
        {
            var tref = _r.GetReference(((RoslynType) type).TypeRef);
            var field = new FieldDefUser(name, new FieldSig(tref.ToTypeSig()));
            field.IsStatic = isStatic;
            field.Access |= (isPublic) ? FieldAttributes.Public : FieldAttributes.Private;
            field.DeclaringType = TypeDef;

            var dfield = new RoslynFieldBuilder(_ts, field);

            _fields.Add(dfield);

            return dfield;
        }

        public RoslynMethodBuilder DefineMethod(string name, bool isPublic, bool isStatic, bool isInterfaceImpl,
            RoslynMethod overrideMethod = null, bool isVirtual = false)
        {
            MethodSig sig;

            if (isStatic)
                sig = MethodSig.CreateStatic(_ts.GetSystemBindings().Void.GetRef().ToTypeSig());
            else
                sig = MethodSig.CreateInstance(_ts.GetSystemBindings().Void.GetRef().ToTypeSig());

            var method = new MethodDefUser(name, sig);

            var dm = new RoslynMethodBuilder(_ts, method, TypeRef);
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

        public RoslynPropertyBuilder DefineProperty(RoslynType propertyType, string name, bool isStatic = false)
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

            var propertyBuilder = new RoslynPropertyBuilder(_ts, prop, TypeRef);
            _properties.Add(propertyBuilder);

            return propertyBuilder;
        }

        public RoslynConstructorBuilder DefineConstructor(bool isStatic, params RoslynType[] args)
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

            var dc = new RoslynConstructorBuilder(_ts, c, TypeRef);


            foreach (var arg in args)
            {
                dc.DefineParameter(arg);
                //  sig.Params.Add(arg.GetRef().ToTypeSig());
            }

            _constructors.Add(dc);

            return dc;
        }

        public RoslynTypeBuilder DefineNastedType(RoslynType baseType, string name, bool isPublic)
        {
            throw new NotImplementedException();
        }

        public void SetCustomAttribute(ICustomAttribute attr)
        {
            var dnlibAttr = (RoslynCustomAttribute) attr;
            dnlibAttr.ImportAttribute(TypeDef.Module);
            ((List<RoslynCustomAttribute>) CustomAttributes).Add(dnlibAttr);
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