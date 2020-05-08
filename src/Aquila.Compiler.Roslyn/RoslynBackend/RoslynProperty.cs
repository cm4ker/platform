using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using dnlib.DotNet;

namespace Aquila.Compiler.Roslyn.RoslynBackend
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class RoslynProperty
    {
        private readonly RoslynTypeSystem _ts;
        protected readonly PropertyDef PropertyDef;
        internal ITypeDefOrRef DeclaringTypeInternal { get; }

        public RoslynType DeclaringType => _cr.GetType(DeclaringTypeInternal);

        private RoslynContextResolver _cr;
        protected RoslynMethod _getter;
        protected RoslynMethod _setter;
        private List<RoslynCustomAttribute> _customAttributes;

        public RoslynProperty(RoslynTypeSystem typeSystem, PropertyDef property, ITypeDefOrRef declaringTypeInternal)
        {
            _ts = typeSystem;
            PropertyDef = property;
            DeclaringTypeInternal = declaringTypeInternal;

            _cr = new RoslynContextResolver(_ts, PropertyDef.Module);
        }

        public bool Equals(RoslynProperty other)
        {
            throw new NotImplementedException();
        }

        public string Name => PropertyDef.Name;

        public RoslynType PropertyType => _cr.GetType(PropertyDef.PropertySig.RetType);

        public RoslynMethod CalculateMethod(MethodDef x)
        {
            return new RoslynMethod(_ts,
                new MemberRefUser(x.Module, x.Name, _cr.ResolveMethodSig(x.MethodSig, null),
                    DeclaringTypeInternal),
                x, DeclaringTypeInternal);
        }

        public virtual RoslynMethod Getter => _getter ??=
            (PropertyDef.GetMethod == null) ? null : CalculateMethod(PropertyDef.GetMethod);

        public virtual RoslynMethod Setter => _setter ??=
            (PropertyDef.SetMethod == null)
                ? null
                : new RoslynMethod(_ts, PropertyDef.SetMethod, PropertyDef.SetMethod, PropertyDef.DeclaringType);

        public IReadOnlyList<RoslynCustomAttribute> CustomAttributes =>
            _customAttributes ??= new List<RoslynCustomAttribute>();

        public RoslynCustomAttribute FindCustomAttribute(RoslynType type)
        {
            return _customAttributes?.FirstOrDefault(x => x.AttributeType == type);
        }
    }
}