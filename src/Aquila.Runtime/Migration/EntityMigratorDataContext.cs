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
        /// <param name="oldCollectionState"></param>
        /// <param name="actualCollectionState"></param>
        public EntityMigratorDataContext(DatabaseRuntimeContext runtimeContext,
            EntityMetadataCollection oldCollectionState, EntityMetadataCollection actualCollectionState
            , DataConnectionContext connectionContext)
        {
            RuntimeContext = runtimeContext;
            OldCollectionState = oldCollectionState;
            ActualCollectionState = actualCollectionState;
            ConnectionContext = connectionContext;
        }

        /// <summary>
        /// For this context we create the migration plan
        /// </summary>
        public DatabaseRuntimeContext RuntimeContext { get; set; }

        /// <summary>
        /// We migrate form this schema
        /// </summary>
        public EntityMetadataCollection OldCollectionState { get; set; }


        /// <summary>
        /// We migrate to this schema
        /// </summary>
        public EntityMetadataCollection ActualCollectionState { get; set; }

        public DataConnectionContext ConnectionContext { get; }
    }
}