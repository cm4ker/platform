using System;
using Aquila.Configuration.Contracts.TypeSystem;

namespace Aquila.Configuration.Common.TypeSystem
{
    public class MetadataRow : IMetadataRow
    {
        public Guid Id { get; set; }

        public Guid ParentId { get; set; }

        public object Metadata { get; set; }
    }
}