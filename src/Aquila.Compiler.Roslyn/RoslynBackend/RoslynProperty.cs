using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Aquila.Compiler.Contracts;
using dnlib.DotNet;
using ICustomAttribute = Aquila.Compiler.Contracts.ICustomAttribute;
using IMethod = Aquila.Compiler.Contracts.IMethod;
using IType = Aquila.Compiler.Contracts.IType;

namespace Aquila.Compiler.Roslyn.RoslynBackend
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class RoslynProperty : IProperty
    {
        private readonly RoslynTypeSystem _ts;
        protected readonly PropertyDef PropertyDef;
        internal ITypeDefOrRef DeclaringTypeInternal { get; }

        public RoslynType DeclaringType => _cr.GetType(DeclaringTypeInternal);

        private RoslynContextResolver _cr;
        private RoslynMethod _getter;
        private RoslynMethod _setter;
        private List<RoslynCustomAttribute> _customAttributes;

        public RoslynProperty(RoslynTypeSystem typeSystem, PropertyDef property, ITypeDefOrRef declaringTypeInternal)
        {
            _ts = typeSystem;
            PropertyDef = property;
            DeclaringTypeInternal = declaringTypeInternal;

            _cr = new RoslynContextResolver(_ts, PropertyDef.Module);
        }

        protected bool Equals(RoslynProperty other)
        {
            throw new NotImplementedException();
        }

        public string Name => PropertyDef.Name;

        public IType PropertyType => _cr.GetType(PropertyDef.PropertySig.RetType);

        public RoslynMethod CalculateMethod(MethodDef x)
        {
            return new RoslynMethod(_ts,
                new MemberRefUser(x.Module, x.Name, _cr.ResolveMethodSig(x.MethodSig, null),
                    DeclaringTypeInternal),
                x, DeclaringTypeInternal);
        }

        public virtual IMethod Getter => _getter ??=
            (PropertyDef.GetMethod == null) ? null : CalculateMethod(PropertyDef.GetMethod);

        public virtual IMethod Setter => _setter ??=
            (PropertyDef.SetMethod == null)
                ? null
                : new RoslynMethod(_ts, PropertyDef.SetMethod, PropertyDef.SetMethod, PropertyDef.DeclaringType);

        public IReadOnlyList<ICustomAttribute> CustomAttributes =>
            _customAttributes ??= new List<RoslynCustomAttribute>();

        public ICustomAttribute FindCustomAttribute(IType type)
        {
            return _customAttributes?.FirstOrDefault(x => x.AttributeType == type);
        }

        public bool Equals(IProperty other)
        {
            throw new NotImplementedException();
        }
    }
}