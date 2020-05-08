using System;
using System.Collections.Generic;

namespace Aquila.Compiler.Contracts
{
    public interface IProperty : IEquatable<IProperty>, IMember
    {
        IType PropertyType { get; }


        IMethod Getter { get; }
        IMethod Setter { get; }

        IReadOnlyList<ICustomAttribute> CustomAttributes { get; }

        ICustomAttribute FindCustomAttribute(IType type);
    }

    /// <summary>
    /// Builder for the property
    /// </summary>
    public interface IPropertyBuilder : IProperty
    {
        IPropertyBuilder WithSetter(IMethod method);

        IPropertyBuilder WithGetter(IMethod method);

        void SetAttribute(ICustomAttribute attr);
    }
}