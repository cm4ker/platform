using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Transactions;
using dnlib.DotNet;
using ZenPlatform.Compiler.Contracts;
using IAssembly = ZenPlatform.Compiler.Contracts.IAssembly;
using ICustomAttribute = ZenPlatform.Compiler.Contracts.ICustomAttribute;
using IField = ZenPlatform.Compiler.Contracts.IField;
using IMethod = ZenPlatform.Compiler.Contracts.IMethod;
using IType = ZenPlatform.Compiler.Contracts.IType;

namespace ZenPlatform.Compiler.Dnlib
{
    [DebuggerDisplay("{" + nameof(TypeRef) + "}")]
    public class DnlibType : IType
    {
        private readonly DnlibTypeSystem _ts;
        private readonly DnlibAssembly _assembly;

        public DnlibType(DnlibTypeSystem typeSystem, TypeDef typeDef, ITypeDefOrRef typeRef, DnlibAssembly assembly)
        {
            _ts = typeSystem;
            _assembly = assembly;

            TypeRef = typeRef ?? throw new ArgumentNullException(nameof(typeRef));
            TypeDef = typeDef ?? typeRef.ResolveTypeDef();
        }

        public TypeDef TypeDef { get; }

        public ITypeDefOrRef TypeRef { get; }

        public bool Equals(IType other)
        {
            return new SigComparer().Equals(TypeRef, ((DnlibType) other).TypeRef);
        }

        public object Id => TypeDef.FullName;
        public string Name => TypeDef.Name;
        public string Namespace => TypeDef.Namespace;
        public string FullName => TypeDef.FullName;
        public IAssembly Assembly => _assembly;

        private IReadOnlyList<IProperty> _properties;
        private IReadOnlyList<IField> _fields;
        private IReadOnlyList<IMethod> _methods;
        private IReadOnlyList<IConstructor> _constructors;
        private IReadOnlyList<IType> _interfaces;

        public IReadOnlyList<IProperty> Properties =>
            _properties ??= TypeDef.Properties.Select(x => new DnlibProperty(_ts, x)).ToList();

        public IReadOnlyList<IField> Fields =>
            _fields ??= TypeDef.Fields.Select(x => new DnlibField(x)).ToList();

        public IReadOnlyList<IEventInfo> Events { get; }

        public IReadOnlyList<IMethod> Methods =>
            _methods ??= TypeDef.Methods.Where(x => !x.IsConstructor)
                .Select(x => (IMethod) new DnlibMethod(_ts, x, x, TypeRef)).ToList();

        public IReadOnlyList<IConstructor> Constructors =>
            _constructors ??= TypeDef.FindConstructors().Select(x =>
                (IConstructor) new DnlibConstructor(_ts,
                    new MemberRefUser(x.Module, x.Name, x.MethodSig, TypeRef) {MethodSig = x.MethodSig}, x,
                    TypeRef)).ToList();

        public IReadOnlyList<ICustomAttribute> CustomAttributes { get; }
        public IReadOnlyList<IType> GenericArguments { get; }

        public IReadOnlyList<IType> Interfaces =>
            _interfaces ??= TypeDef.Interfaces
                .Where(x => x.Interface.ResolveTypeDef() != null).Select(x =>
                {
                    return new DnlibType(_ts, x.Interface.ResolveTypeDef(),
                        (x.Interface.IsTypeRef) ? (TypeRef) x.Interface : x.Interface.ToTypeRef(),
                        (DnlibAssembly) _ts.FindAssembly(x.Interface.ResolveTypeDef().DefinitionAssembly.FullName));
                })
                .ToList();

        public bool IsAssignableFrom(IType type)
        {
            if (type.IsValueType
                && GenericTypeDefinition?.FullName == "System.Nullable`1"
                && GenericArguments[0].Equals(type))
                return true;
            if (FullName == "System.Object" && type.IsInterface)
                return true;
            var baseType = type;
            while (baseType != null)
            {
                if (baseType.Equals(this))
                    return true;
                baseType = baseType.BaseType;
            }

            if (IsInterface && type.GetAllInterfaces().Any(IsAssignableFrom))
                return true;
            return false;
        }

        public IType MakeGenericType(IReadOnlyList<IType> typeArguments)
        {
            throw new NotImplementedException();
        }

        public IType GenericTypeDefinition { get; }

        public bool IsArray => TypeRef.ToTypeSig().IsSingleOrMultiDimensionalArray;

        public IType ArrayElementType => _ts.Resolve(TypeRef.ScopeType);

        public IType MakeArrayType()
        {
            return new DnlibType(_ts, null, new TypeSpecUser(new SZArraySig(TypeRef.ToTypeSig())), _assembly);
        }

        public IType MakeArrayType(int dimensions)
        {
            return new DnlibType(_ts, null, new TypeSpecUser(new ArraySig(TypeRef.ToTypeSig(), dimensions)), _assembly);
        }

        public IType BaseType => _ts.Resolve(TypeDef.BaseType);

        public bool IsValueType => TypeDef.IsValueType;

        public bool IsEnum => TypeDef.IsEnum;

        public bool IsInterface => TypeDef.IsInterface;

        public bool IsSystem { get; }

        public bool IsPrimitive => TypeDef.IsPrimitive;

        public IType GetEnumUnderlyingType()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<IType> GenericParameters { get; }
    }
}