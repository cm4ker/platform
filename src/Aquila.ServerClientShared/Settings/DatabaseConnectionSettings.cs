using System;
using System.Collections.Generic;
using System.Text;
using Aquila.Core.Assemlies;

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
