using System;
using System.Collections.Generic;
using dnlib.DotNet;
using Aquila.Compiler.Contracts;
using ICustomAttribute = Aquila.Compiler.Contracts.ICustomAttribute;
using IMethod = Aquila.Compiler.Contracts.IMethod;

namespace Aquila.Compiler.Dnlib
{
    public class DnlibPropertyBuilder : DnlibProperty, IPropertyBuilder
    {
        public IPropertyBuilder WithSetter(IMethod method)
        {
            _setter = method;
            PropertyDef.SetMethod = ((DnlibMethodBase) method).MethodDef;
            return this;
        }

        public IPropertyBuilder WithGetter(IMethod method)
        {
            _getter = method;
            PropertyDef.GetMethod = ((DnlibMethodBase) method).MethodDef;
            return this;
        }

        public void SetAttribute(ICustomAttribute attr)
        {
            var dnlibAttr = (DnlibCustomAttribute) attr;
            dnlibAttr.ImportAttribute(PropertyDef.Module);
            PropertyDef.CustomAttributes.Add(dnlibAttr.GetCA());
            ((List<DnlibCustomAttribute>) CustomAttributes).Add(dnlibAttr);
        }

        public DnlibPropertyBuilder(DnlibTypeSystem ts, PropertyDef property, ITypeDefOrRef declaringType) : base(ts,
            property, declaringType)
        {
        }
    }
}