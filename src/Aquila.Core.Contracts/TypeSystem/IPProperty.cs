using System;
using System.Collections.Generic;

namespace Aquila.Core.Contracts.TypeSystem
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

        IPType Type { get; }

        void SetType(Guid guid);
    }
}