using System;
using dnlib.DotNet;
using ZenPlatform.Compiler.Contracts;
using ICustomAttribute = ZenPlatform.Compiler.Contracts.ICustomAttribute;
using IMethod = ZenPlatform.Compiler.Contracts.IMethod;

namespace ZenPlatform.Compiler.Dnlib
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
            throw new NotImplementedException();
        }

        public DnlibPropertyBuilder(DnlibTypeSystem ts, PropertyDef property) : base(ts, property)
        {
        }
    }
}