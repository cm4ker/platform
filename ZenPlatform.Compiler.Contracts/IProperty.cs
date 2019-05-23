using System;
using System.Collections.Generic;

namespace ZenPlatform.Compiler.Contracts
{
    public interface IProperty : IEquatable<IProperty>, IMember
    {
        IType PropertyType { get; }

        IMethod Setter { get; }

        IMethod Getter { get; }

        IReadOnlyList<ICustomAttribute> CustomAttributes { get; }
    }

    /// <summary>
    /// Builder for the property
    /// </summary>
    public interface IPropertyBuilder : IProperty
    {
        IPropertyBuilder WithPropType(IType propertyType);

        IPropertyBuilder WithSetter(IMethod method);

        IPropertyBuilder WithGetter(IMethod method);
    }
}