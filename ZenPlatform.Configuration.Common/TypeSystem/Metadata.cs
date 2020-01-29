using System;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Configuration.Common.TypeSystem
{
    public class Metadata : IMetadata
    {
        
    }

    public class MaterializedMetadataRow
    {
        public string Uri { get; set; }

        public Guid ComponentId { get; set; }

        public Metadata Object { get; set; }
    }
}