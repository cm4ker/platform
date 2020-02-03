using System;
using System.Collections.Generic;

namespace ZenPlatform.Configuration.Contracts.TypeSystem
{
    public interface IProperty : ITypeManagerProvider
    {
        Guid Id { get; set; }
        Guid ParentId { get; set; }
        string Name { get; set; }
        bool IsSelfLink { get; set; }
        bool IsSystem { get; set; }
        bool IsUnique { get; set; }

        bool IsReadOnly { get; set; }
        IEnumerable<IType> Types { get; }
    }
}