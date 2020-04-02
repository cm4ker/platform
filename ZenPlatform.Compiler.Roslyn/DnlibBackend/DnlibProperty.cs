using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using dnlib.DotNet;

namespace ZenPlatform.Compiler.Roslyn.DnlibBackend
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class SreProperty
    {
        private readonly SreTypeSystem _ts;
        protected readonly PropertyDef PropertyDef;
        internal ITypeDefOrRef DeclaringTypeInternal { get; }

        public SreType DeclaringType => _cr.GetType(DeclaringTypeInternal);

        private SreContextResolver _cr;
        protected SreMethod _getter;
        protected SreMethod _setter;
        private List<SreCustomAttribute> _customAttributes;

        public SreProperty(SreTypeSystem typeSystem, PropertyDef property, ITypeDefOrRef declaringTypeInternal)
        {
            _ts = typeSystem;
            PropertyDef = property;
            DeclaringTypeInternal = declaringTypeInternal;

            _cr = new SreContextResolver(_ts, PropertyDef.Module);
        }

        public bool Equals(SreProperty other)
        {
            throw new NotImplementedException();
        }

        public string Name => PropertyDef.Name;

        public SreType PropertyType => _cr.GetType(PropertyDef.PropertySig.RetType);

        public SreMethod CalculateMethod(MethodDef x)
        {
            return new SreMethod(_ts,
                new MemberRefUser(x.Module, x.Name, _cr.ResolveMethodSig(x.MethodSig, null),
                    DeclaringTypeInternal),
                x, DeclaringTypeInternal);
        }

        public SreMethod Getter => _getter ??=
            (PropertyDef.GetMethod == null) ? null : CalculateMethod(PropertyDef.GetMethod);

        public SreMethod Setter => _setter ??=
            (PropertyDef.SetMethod == null)
                ? null
                : new SreMethod(_ts, PropertyDef.SetMethod, PropertyDef.SetMethod, PropertyDef.DeclaringType);

        public IReadOnlyList<SreCustomAttribute> CustomAttributes =>
            _customAttributes ??= new List<SreCustomAttribute>();

        public SreCustomAttribute FindCustomAttribute(SreType type)
        {
            return _customAttributes?.FirstOrDefault(x => x.AttributeType == type);
        }
    }
}