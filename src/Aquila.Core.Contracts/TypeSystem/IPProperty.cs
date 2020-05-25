using System;
using System.Collections.Generic;

namespace Aquila.Core.Contracts.TypeSystem
{
    public interface IPField : IPMember
    {
    }

    public interface IPProperty : IPMember
    {
        bool IsSelfLink { get; set; }
        bool IsSystem { get; set; }
        bool IsUnique { get; set; }
        bool IsReadOnly { get; set; }
        IPType Type { get; }

        //TODO: We can organize two worlds Mutable and Immutable where we have builders and just ReadOnly objects
        void SetType(Guid guid);
    }


    public interface IPUniqueObject : ITypeManagerProvider
    {
        Guid Id { get; }
    }

    public interface IPMember : IPUniqueObject
    {
        Guid ParentId { get; }

        string Name { get; set; }

        bool IsProperty { get; }

        bool IsMethod { get; }

        bool IsConstructor { get; }
    }
}