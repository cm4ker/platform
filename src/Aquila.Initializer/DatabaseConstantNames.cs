namespace Aquila.Initializer
{
    public static class DatabaseConstantNames
    {
        #region User table

        /// <summary>
        /// User table name
        /// </summary>
        public const string USER_TABLE_NAME = "users";

        /// <summary>
        /// User id field
        /// </summary>
        public const string USER_TABLE_ID_FIELD = "user_id";

        /// <summary>
        /// User name field
        /// </summary>
        public const string USER_TABLE_NAME_FIELD = "user_name";

        /// <summary>
        /// user password
        /// </summary>
        public const string USER_TABLE_PASSWORD_FIELD = "user_password";

        #endregion

        #region Config table

        /// <summary>
        /// Metadata table name
        /// </summary>
        public const string MD_TABLE_NAME = "metadata";


        /// <summary>
        /// Column witch contains name of blob
        /// </summary>
        public const string MD_TABLE_BLOB_NAME_FIELD = "blob_name";

        /// <summary>
        /// Column with data
        /// </summary>
        public const string MD_TABLE_DATA_FIELD = "data";

        #endregion


        #region SaveConfig table

        /// <summary>
        /// Pending configuration for migration
        /// </summary>
        public const string PEND_MD_TABLE_NAME = "metadata_pending";

        #endregion

        #region Migration status table

        public const string MIGRATION_STATUS_TABLE_NAME = "migration_status";

        public const string MIGRATION_STATUS_TABLE_CHANGE_NAME_FIELD = "change_table";
        public const string MIGRATION_STATUS_TABLE_RENAME_NAME_FIELD = "rename_table";
        public const string MIGRATION_STATUS_TABLE_DELETE_NAME_FIELD = "delete_table";
        public const string MIGRATION_STATUS_TABLE_COPY_NAME_FIELD = "copy_table";

        public const string MIGRATION_STATUS_TABLE_ORIGINAL_NAME_FIELD = "original_table";
        public const string MIGRATION_STATUS_TABLE_TEMP_NAME_FIELD = "temp_table";

        #endregion
    }
}