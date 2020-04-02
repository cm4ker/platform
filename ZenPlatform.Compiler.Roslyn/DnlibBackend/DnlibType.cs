using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using dnlib.DotNet;

namespace ZenPlatform.Compiler.Roslyn.DnlibBackend
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class SreType
    {
        private readonly SreTypeSystem _ts;
        private readonly SreAssembly _assembly;
        private readonly SreContextResolver _cr;

        public SreType(SreTypeSystem typeSystem, TypeDef typeDef, ITypeDefOrRef typeRef, SreAssembly assembly)
        {
            _ts = typeSystem;
            _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));

            TypeRef = typeRef ?? throw new ArgumentNullException(nameof(typeRef));
            TypeDef = typeDef ?? typeRef.ResolveTypeDef();

            if (typeDef is null) throw new ArgumentNullException(nameof(typeDef));
            _cr = new SreContextResolver(_ts, typeDef.Module);
        }

        public TypeDef TypeDef { get; }

        public ITypeDefOrRef TypeRef { get; }

        public bool Equals(SreType other)
        {
            return new SigComparer().Equals(TypeRef, ((SreType) other)?.TypeRef);
        }

        public SreTypeSystem TypeSystem => _ts;

        public object Id => TypeDef.FullName;
        public string Name => TypeRef.Name;
        public string Namespace => TypeRef.Namespace;
        public string FullName => TypeRef.FullName;
        public SreAssembly Assembly => _assembly;

        private IReadOnlyList<SreProperty> _properties;
        private IReadOnlyList<SreField> _fields;
        private IReadOnlyList<SreMethod> _methods;
        private IReadOnlyList<SreConstructor> _constructors;
        private IReadOnlyList<SreType> _interfaces;
        private IReadOnlyList<SreType> _genericArguments;
        private IReadOnlyList<ICustomAttribute> _customAttributes;

        public virtual IReadOnlyList<SreProperty> Properties =>
            _properties ??= TypeDef.Properties.Select(x => new SreProperty(_ts, x, TypeRef)).ToList();

        public virtual IReadOnlyList<SreField> Fields =>
            _fields ??= TypeDef.Fields.Select(x => new SreField(_ts, x)).ToList();

        public virtual IReadOnlyList<SreMethod> Methods => _methods ??= CalculateMethods();

        public virtual IReadOnlyList<SreConstructor> Constructors =>
            _constructors ??= TypeDef.FindConstructors().Select(x =>
            {
                return (SreConstructor) new SreConstructor(_ts,
                    new MemberRefUser(x.Module, x.Name, x.MethodSig, TypeRef), x,
                    TypeRef);
            }).ToList();


        public SreMethod CalculateMethod(MethodDef x)
        {
            return new SreMethod(_ts,
                new MemberRefUser(x.Module, x.Name, _cr.ResolveMethodSig(x.MethodSig, GenericArguments?.ToArray()),
                    TypeRef),
                x, TypeRef);
        }

        private List<SreMethod> CalculateMethods()
        {
            return TypeDef.Methods.Where(x => !x.IsConstructor)
                .Select(CalculateMethod)
                .ToList();
        }

        public virtual IReadOnlyList<ICustomAttribute> CustomAttributes =>
            _customAttributes ??= new List<ICustomAttribute>();

        public virtual IReadOnlyList<SreType> GenericArguments =>
            _genericArguments ??= (TypeRef as TypeSpec)?.TryGetGenericInstSig()?.GenericArguments?
                .Select(x => _cr.GetType(x)).ToList();

        public virtual IReadOnlyList<SreType> Interfaces =>
            _interfaces ??= TypeDef.Interfaces
                .Where(x => x.Interface.ResolveTypeDef() != null).Select(x =>
                {
                    return new SreType(_ts, x.Interface.ResolveTypeDef(),
                        (x.Interface.IsTypeRef) ? (TypeRef) x.Interface : x.Interface.ToTypeRef(),
                        (SreAssembly) _ts.FindAssembly(x.Interface.ResolveTypeDef().DefinitionAssembly.FullName));
                })
                .ToList();

        public SreType MakeGenericType(IReadOnlyList<SreType> typeArguments)
        {
            if (TypeRef is TypeDef || TypeRef is TypeRef)
            {
                var typeSig = TypeRef.ToTypeSig();

                var sig = new GenericInstSig(typeSig.ToClassOrValueTypeSig(), typeArguments
                    .Select(x => ((SreType) x).TypeRef.ToTypeSig())
                    .ToArray());

                var generic = new TypeSpecUser(sig);

                return new SreType(_ts, TypeDef, generic, _assembly);
            }

            throw new Exception("Can't create generic Type");
        }

        public SreType GenericTypeDefinition { get; }

        public bool IsArray => TypeRef.ToTypeSig().IsSingleOrMultiDimensionalArray;

        public SreType ArrayElementType => _ts.Resolve(TypeRef.ScopeType);

        public SreType MakeArrayType()
        {
            return new SreType(_ts, TypeDef, new TypeSpecUser(new SZArraySig(TypeRef.ToTypeSig())), _assembly);
        }

        public SreType MakeArrayType(int dimensions)
        {
            return new SreType(_ts, TypeDef, new TypeSpecUser(new ArraySig(TypeRef.ToTypeSig(), dimensions)),
                _assembly);
        }

        public SreType BaseType => (TypeDef.BaseType != null) ? _ts.Resolve(TypeDef.BaseType) : null;

        public bool IsValueType => TypeDef.IsValueType;

        public bool IsEnum => TypeDef.IsEnum;

        public bool IsInterface => TypeDef.IsInterface;

        public bool IsSystem => false;

        public bool IsPrimitive => TypeDef.IsPrimitive;


        public bool IsPublic => TypeDef.IsPublic;

        public bool IsAbstract => TypeDef.IsAbstract;

        public bool IsGeneric => TypeRef.ToTypeSig() is GenericInstSig;

        public SreType GetEnumUnderlyingType()
        {
            throw new NotImplementedException();
        }

        public virtual void DumpRef(TextWriter tw)
        {
            if (FullName == "System.Void")
            {
                tw.W("void");
                return;
            }

            if (IsArray)
            {
                
            }

            if (IsGeneric)
            {
                tw.Write(GetNameWithoutGenericArity(FullName));
                using (tw.AngleBrace())
                {
                    var wasFirst = false;
                    foreach (var arg in GenericArguments)
                    {
                        if (wasFirst)
                            tw.W(",");

                        arg.DumpRef(tw);

                        wasFirst = true;
                    }
                }
            }
            else
                tw.Write(FullName);
        }

        private string GetNameWithoutGenericArity(string name)
        {
            int index = name.IndexOf('`');
            return index == -1 ? name : name.Substring(0, index);
        }
    }
}