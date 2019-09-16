using System;
using dnlib.DotNet;
using ZenPlatform.Compiler.Contracts;
using IMethod = ZenPlatform.Compiler.Contracts.IMethod;

namespace ZenPlatform.Compiler.Dnlib
{
    public class DnlibPropertyBuilder : DnlibProperty, IPropertyBuilder
    {
        public IPropertyBuilder WithSetter(IMethod method)
        {
            PropertyDef.GetMethod = ((DnlibMethod) method).MethodDef;
            return this;
        }

        public IPropertyBuilder WithGetter(IMethod method)
        {
            PropertyDef.SetMethod = ((DnlibMethod) method).MethodDef;
            return this;
        }

        public DnlibPropertyBuilder(PropertyDef property) : base(property)
        {
        }
    }
}