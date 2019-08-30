using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Core.Assemlies;

namespace ZenPlatform.Core.Settings
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

    public class AssemblyCache
    {
        public AssemblyDescription Description;
        public string FilePath;
    }

    public class AssemblesCache 
    {
        public List<AssemblyCache> Assemblies;

        public AssemblesCache()
        {
            Assemblies = new List<AssemblyCache>();
        }

    }


}
