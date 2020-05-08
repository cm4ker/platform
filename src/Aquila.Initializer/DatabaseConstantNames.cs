namespace Aquila.Initializer
{
    public static class DatabaseConstantNames
    {
        #region User table

        /// <summary>
        /// Имя таблицы пользователей
        /// </summary>
        public const string USER_TABLE_NAME = "users";

        /// <summary>
        /// Имя колонки идентификатора
        /// </summary>
        public const string USER_TABLE_ID_FIELD = "user_id";

        /// <summary>
        /// Имя колонки имени пользователя
        /// </summary>
        public const string USER_TABLE_NAME_FIELD = "user_name";

        /// <summary>
        /// Имя колонки пароль пользователя
        /// </summary>
        public const string USER_TABLE_PASSWORD_FIELD = "user_password";

        #endregion

        #region Config table

        /// <summary>
        /// Имя таблицы конфигурации
        /// </summary>
        public const string CONFIG_TABLE_NAME = "config";


        /// <summary>
        /// Имя столбца с именем данных
        /// </summary>
        public const string CONFIG_TABLE_BLOB_NAME_FIELD = "blob_name";

        /// <summary>
        /// Имя столбца с данными
        /// </summary>
        public const string CONFIG_TABLE_DATA_FIELD = "data";

        #endregion


        #region SaveConfig table

        /// <summary>
        /// Имя таблицы непременённой конфигурации
        /// </summary>
        public const string SAVE_CONFIG_TABLE_NAME = "config_save";

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