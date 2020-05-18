using System;

namespace Aquila.Core.Contracts.TypeSystem
{
    public interface IMetadataRow
    {
        Guid Id { get; set; }
        Guid ParentId { get; set; }
        object Metadata { get; set; }
    }
}