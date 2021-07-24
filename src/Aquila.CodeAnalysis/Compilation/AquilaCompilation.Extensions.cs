using System.Collections.Generic;
using Aquila.Metadata;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis
{
    public static class AquilaCompilationExtensions
    {
        public static AquilaCompilation AddMetadata(this Compilation compilation,
            params EntityMetadata[] metadatas)
        {
            return ((AquilaCompilation)compilation).AddMetadata(metadatas);
        }

        public static AquilaCompilation AddMetadata(this Compilation compilation,
            IEnumerable<EntityMetadata> metadatas)
        {
            return ((AquilaCompilation)compilation).AddMetadata(metadatas);
        }
    }
}