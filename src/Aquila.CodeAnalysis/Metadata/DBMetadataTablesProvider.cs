using System;
using System.Collections.Generic;
using Aquila.Metadata;

namespace Aquila.Syntax.Metadata
{
    /// <summary>
    /// Solution. For translate query for real database we need get table names and etc
    /// Database has information about this. We Query it and create the provider for underlying algorithms
    /// </summary>
    class DBMetadataTablesProvider
    {
        private readonly EntityMetadata _md;
        /*
            1) Translate type to table 
         */

        public DBMetadataTablesProvider(EntityMetadata md, object global_metadata_reader)
        {
            _md = md;
        }

        List<TableInfo> Translate()
        {
            throw new NotImplementedException();
        }

        public class TableInfo
        {
            public TableInfo(EntityMetadata md, string tableName)
            {
            }
        }
    }
}