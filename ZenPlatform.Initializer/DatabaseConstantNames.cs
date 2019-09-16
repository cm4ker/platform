namespace ZenPlatform.Initializer
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
    }
}