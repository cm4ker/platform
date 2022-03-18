using System.Collections.Generic;

namespace Aquila.Core.Settings
{
    public class PlatformClientSettings
    {
        public List<DatabaseConnectionSettings> Databases;

        public PlatformClientSettings()
        {
            Databases = new List<DatabaseConnectionSettings>();
        }
    }

    public class DatabaseConnectionSettings
    {
        public string Address;
        public string Database;
    }
}