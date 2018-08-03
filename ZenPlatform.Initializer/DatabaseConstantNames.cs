namespace ZenPlatform.Initializer
{
    public static class DatabaseConstantNames
    {
        #region User table

        public const string USER_TABLE_NAME = "users";
        public const string USER_TABLE_ID_FIELD = "id";
        public const string USER_TABLE_NAME_FIELD = "name";

        #endregion

        #region Config table

        public const string CONFIG_TABLE_NAME = "config";
        public const string CONFIG_TABLE_BLOB_NAME_FIELD = "blob_name";
        public const string CONFIG_TABLE_DATA_FIELD = "data";

        #endregion


        #region SaveConfig table

        public const string SAVE_CONFIG_TABLE_NAME = "config_save";

        #endregion
    }
}