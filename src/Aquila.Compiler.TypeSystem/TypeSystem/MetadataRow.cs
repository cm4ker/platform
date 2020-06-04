using System;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    public class MetadataRow
    {
        public Guid Id { get; set; }

        public Guid ParentId { get; set; }

        public object Metadata { get; set; }
    }
}