using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using dnlib.DotNet;
using ZenPlatform.Compiler.Contracts;
using ICustomAttribute = ZenPlatform.Compiler.Contracts.ICustomAttribute;
using IMethod = ZenPlatform.Compiler.Contracts.IMethod;
using IType = ZenPlatform.Compiler.Contracts.IType;

namespace ZenPlatform.Compiler.Dnlib
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class DnlibProperty : IProperty
    {
        private readonly DnlibTypeSystem _ts;
        protected readonly PropertyDef PropertyDef;
        public ITypeDefOrRef DeclaringType { get; }
        private DnlibContextResolver _cr;
        protected IMethod _getter;
        protected IMethod _setter;
        private List<DnlibCustomAttribute> _customAttributes;

        public DnlibProperty(DnlibTypeSystem typeSystem, PropertyDef property, ITypeDefOrRef declaringType)
        {
            _ts = typeSystem;
            PropertyDef = property;
            DeclaringType = declaringType;

            _cr = new DnlibContextResolver(_ts, PropertyDef.Module);
        }

        public bool Equals(IProperty other)
        {
            throw new NotImplementedException();
        }

        public string Name => PropertyDef.Name;

        public IType PropertyType => _cr.GetType(PropertyDef.PropertySig.RetType);

        public IMethod CalculateMethod(MethodDef x)
        {
            return new DnlibMethod(_ts,
                new MemberRefUser(x.Module, x.Name, _cr.ResolveMethodSig(x.MethodSig, null),
                    DeclaringType),
                x, DeclaringType);
        }

        public IMethod Getter => _getter ??=
            (PropertyDef.GetMethod == null) ? null : CalculateMethod(PropertyDef.GetMethod);

        public IMethod Setter => _setter ??=
            (PropertyDef.SetMethod == null)
                ? null
                : new DnlibMethod(_ts, PropertyDef.SetMethod, PropertyDef.SetMethod, PropertyDef.DeclaringType);

        public IReadOnlyList<ICustomAttribute> CustomAttributes =>
            _customAttributes ??= new List<DnlibCustomAttribute>();

        public ICustomAttribute FindCustomAttribute(IType type)
        {
            return _customAttributes?.FirstOrDefault(x => x.AttributeType == type);
        }
    }
}