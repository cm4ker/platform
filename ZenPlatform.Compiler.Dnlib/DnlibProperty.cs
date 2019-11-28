using System;
using System.Collections.Generic;
using dnlib.DotNet;
using ZenPlatform.Compiler.Contracts;
using ICustomAttribute = ZenPlatform.Compiler.Contracts.ICustomAttribute;
using IMethod = ZenPlatform.Compiler.Contracts.IMethod;
using IType = ZenPlatform.Compiler.Contracts.IType;

namespace ZenPlatform.Compiler.Dnlib
{
    public class DnlibProperty : IProperty
    {
        private readonly DnlibTypeSystem _ts;
        protected readonly PropertyDef PropertyDef;
        private DnlibContextResolver _cr;

        public DnlibProperty(DnlibTypeSystem typeSystem, PropertyDef property)
        {
            _ts = typeSystem;
            PropertyDef = property;

            _cr = new DnlibContextResolver(_ts, PropertyDef.Module);
        }

        public bool Equals(IProperty other)
        {
            throw new NotImplementedException();
        }

        public string Name => PropertyDef.Name;

        public IType PropertyType => _cr.GetType(PropertyDef.DeclaringType.ToTypeSig());
        public IMethod Getter { get; }
        public IMethod Setter { get; }
        public IReadOnlyList<ICustomAttribute> CustomAttributes { get; }
    }
}