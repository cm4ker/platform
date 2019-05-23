using System;
using System.Collections.Generic;

namespace ZenPlatform.Compiler.Contracts
{
    public interface IProperty : IEquatable<IProperty>, IMember
    {
        IType PropertyType { get; }

        IReadOnlyList<ICustomAttribute> CustomAttributes { get; }
    }

    /// <summary>
    /// Builder for the property
    /// </summary>
    public interface IPropertyBuilder : IProperty
    {
        IPropertyBuilder WithSetter(IMethod method);

        IPropertyBuilder WithGetter(IMethod method);
    }
}