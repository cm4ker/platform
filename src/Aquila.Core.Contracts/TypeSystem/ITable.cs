using System;
using System.Collections.Generic;

namespace Aquila.Core.Contracts.TypeSystem
{
    public interface ITable : IPMember
    {
        Guid GroupId { get; set; }

        IEnumerable<IPProperty> Properties { get; }
    }
}