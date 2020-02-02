using System;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Configuration.Common.TypeSystem
{
    public class MetadataRow : IMetadataRow
    {
        public Guid Id { get; set; }

        public Guid ParentId { get; set; }

        public object Metadata { get; set; }
    }
}