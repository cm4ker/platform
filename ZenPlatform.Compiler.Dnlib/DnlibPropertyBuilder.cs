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
            throw new NotImplementedException();
        }

        public IPropertyBuilder WithGetter(IMethod method)
        {
            throw new NotImplementedException();
        }

        public DnlibPropertyBuilder(PropertyDef property) : base(property)
        {
        }
    }
}