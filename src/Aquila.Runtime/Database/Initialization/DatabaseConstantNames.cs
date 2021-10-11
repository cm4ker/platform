namespace Aquila.Initializer
{
    public static class DBConstNames
    {
        public static class Metadata
        {
            /// <summary>
            /// Metadata table name
            /// </summary>
            public const string MD_TABLE = "metadata";

            /// <summary>
            /// Pending configuration for migration
            /// </summary>
            public const string PENDING_MD_TABLE = MD_TABLE + "_pending";

            /// <summary>
            /// Column witch contains name of blob
            /// </summary>
            public const string BLOB_NAME_COLUMN = "blob_name";

            /// <summary>
            /// Column with data
            /// </summary>
            public const string DATA_COLUMN = "data";
        }

        public static class Files
        {
            /// <summary>
            /// Files table name
            /// </summary>
            public const string FILES_TABLE = "metadata";

            /// <summary>
            /// Pending files table name
            /// </summary>
            public const string PENDING_FILES_TABLE = FILES_TABLE + "_pending";

            /// <summary>
            /// Pending files table name
            /// </summary>
            public const string TYPE_COLUMN = "type";

            /// <summary>
            /// Pending files table name
            /// </summary>
            public const string NAME_COLUMN = "name";

            /// <summary>
            /// Pending files table name
            /// </summary>
            public const string CREATE_DATETIME_COLUMN = "create_datetime";

            /// <summary>
            /// Pending files table name
            /// </summary>
            public const string DATA_COLUMN = "data";
        }

        public static class Descriptors
        {
            /// <summary>
            /// Descriptors table name
            /// </summary>
            public const string DESCRIPTORS_TABLE = "descriptors";

            /// <summary>
            /// Id column
            /// </summary>
            public const string ID_COLUMN = "id";

            /// <summary>
            /// Metadata name
            /// </summary>
            public const string MD_NAME_COLUMN = "id_s";

            /// <summary>
            /// Database id
            /// </summary>
            public const string DB_ID_COLUMN = "id_n";

            /// <summary>
            /// Database name column
            /// </summary>
            public const string DB_NAME_COLUMN = "db_name";
        }

        public static class MigrationStatus
        {
            public const string MIGRATION_STATUS_TABLE = "migration_status";

            public const string ID_COLUMN = Migration.ID_COLUMN;

            public const string CHANGE_TABLE_COLUMN = "change_table";
            public const string RENAME_TABLE_COLUMN = "rename_table";
            public const string DELETE_TABLE_COLUMN = "delete_table";
            public const string COPY_TABLE_COLUMN = "copy_table";
            public const string DATETIME_COLUMN = "datetime";

            public const string ORIGINAL_TABLE_COLUMN = "original_table";
            public const string TEMP_TABLE_COLUMN = "temp_table";
        }

        public static class Migration
        {
            public const string MIGRATION_TABLE = "migration";

            public const string ID_COLUMN = "migration_id";
            public const string COMPLETED_COLUMN = "completed";
            public const string DATETIME_COLUMN = "datetime";
        }
    }
}