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
        protected readonly PropertyDef PropertyDef;

        public DnlibProperty(PropertyDef property)
        {
            PropertyDef = property;
        }

        public bool Equals(IProperty other)
        {
            throw new NotImplementedException();
        }

        public string Name { get; }
        public IType PropertyType { get; }
        public IMethod Getter { get; }
        public IMethod Setter { get; }
        public IReadOnlyList<ICustomAttribute> CustomAttributes { get; }
    }
}