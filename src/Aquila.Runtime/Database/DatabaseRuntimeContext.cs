using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Unicode;
using Aquila.Data;
using Aquila.Initializer;
using Aquila.Metadata;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Aquila.Runtime
{
    /// <summary>
    /// Database context for already running server
    /// </summary>
    public partial class DatabaseRuntimeContext
    {
        public static DatabaseRuntimeContext CreateAndLoad(DataConnectionContext dcc)
        {
            var drc = new DatabaseRuntimeContext();
            drc.LoadAll(dcc);

            return drc;
        }

        /// <summary>
        /// Creates the instance for database runtime context
        /// </summary>
        public DatabaseRuntimeContext()
        {
            _filesRC = new FilesRC(DBConstNames.Files.FILES_TABLE);
            _pendingFilesRC = new FilesRC(DBConstNames.Files.PENDING_FILES_TABLE);

            _descriptorsRC = new DescriptorsRC();
            _metadataRC = new MetadataRC(DBConstNames.Metadata.MD_TABLE);
            _pendingMetadataRC = new MetadataRC(DBConstNames.Metadata.PENDING_MD_TABLE);
        }

        private FilesRC _filesRC;
        private DescriptorsRC _descriptorsRC;
        private MetadataRC _metadataRC;
        private MetadataRC _pendingMetadataRC;
        private FilesRC _pendingFilesRC;

        public FilesRC Files => _filesRC;

        public FilesRC PendingFiles => _pendingFilesRC;

        public DescriptorsRC Descriptors => _descriptorsRC;
        public MetadataRC Metadata => _metadataRC;
        public MetadataRC PendingMetadata => _pendingMetadataRC;

        /// <summary>
        /// Load runtime information from DB. If db version is not in sync with in-memory then the table rewrite version from db
        /// </summary>
        /// <param name="context"></param>
        public void LoadAll(DataConnectionContext context)
        {
            Descriptors.LoadDescriptors(context);
            Metadata.LoadMetadata(context);
            PendingMetadata.LoadMetadata(context);
        }


        /// <summary>
        /// Store in-memory changes to the db
        /// </summary>
        /// <param name="context"></param>
        public void SaveAll(DataConnectionContext context)
        {
            Descriptors.SaveDescriptors(context);
            Metadata.SaveMetadata(context);
            PendingMetadata.SaveMetadata(context);
        }

        /// <summary>
        /// Apply pending changes from staged tables
        /// </summary>
        /// <param name="dcc"></param>
        public void ApplyPendingChanges(DataConnectionContext dcc)
        {
            dcc.BeginTransaction();
            PendingMetadata.TransferTo(dcc, Metadata);
            PendingFiles.TransferTo(dcc, Files);

            PendingMetadata.Clear(dcc);
            PendingFiles.Clear(dcc);

            dcc.CommitTransaction();
        }
    }
}