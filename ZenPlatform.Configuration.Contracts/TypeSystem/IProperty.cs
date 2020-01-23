using System;

namespace ZenPlatform.Configuration.Contracts.TypeSystem
{
    public interface IProperty
    {
        Guid Id { get; set; }
        Guid ParentId { get; set; }
        string Name { get; set; }
        bool IsSelfLink { get; set; }
        bool IsSystem { get; set; }
        bool IsReadOnly { get; set; }
        Metadata Metadata { get; set; }
    }
}