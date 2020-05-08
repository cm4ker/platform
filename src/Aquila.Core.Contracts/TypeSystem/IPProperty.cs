using System;
using System.Collections.Generic;

namespace Aquila.Configuration.Contracts.TypeSystem
{
    public interface IPProperty : ITypeManagerProvider
    {
        Guid Id { get; set; }
        Guid ParentId { get; set; }
        string Name { get; set; }
        bool IsSelfLink { get; set; }
        bool IsSystem { get; set; }
        bool IsUnique { get; set; }

        bool IsReadOnly { get; set; }
        IEnumerable<IPType> Types { get; }
    }
}