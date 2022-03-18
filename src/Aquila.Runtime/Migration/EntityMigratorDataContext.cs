using Aquila.Data;
using Aquila.Metadata;
using Aquila.Runtime;

namespace Aquila.Migrations
{
    /// <summary>
    /// The data for creation migration plan
    /// </summary>
    public class EntityMigratorDataContext
    {
        /// <summary>
        /// Creates data context for migrator
        /// </summary>
        /// <param name="runtimeContext"></param>
        /// <param name="current"></param>
        /// <param name="pending"></param>
        public EntityMigratorDataContext(DatabaseRuntimeContext runtimeContext, DataConnectionContext connectionContext)
        {
            RuntimeContext = runtimeContext;
            ConnectionContext = connectionContext;
        }

        /// <summary>
        /// For this context we create the migration plan
        /// </summary>
        public DatabaseRuntimeContext RuntimeContext { get; }

        /// <summary>
        /// Connection context
        /// </summary>
        public DataConnectionContext ConnectionContext { get; }

        /// <summary>
        /// We migrate form this schema
        /// </summary>
        public MetadataProvider Current => RuntimeContext.Metadata.GetMetadata();

        /// <summary>
        /// We migrate to this schema
        /// </summary>
        public MetadataProvider Pending => RuntimeContext.PendingMetadata.GetMetadata();
    }
}