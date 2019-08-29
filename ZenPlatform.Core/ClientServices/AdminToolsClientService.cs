using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ZenPlatform.Core.ClientServices;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Network;
using ZenPlatform.Core.Settings;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Core.Tools
{
    public class AdminToolsClientService : IAdminToolsClientService
    {
        private IEnvironmentManager _environmentManager;
        private ISettingsStorage _settingsStorage;

        public AdminToolsClientService(IEnvironmentManager environmentManager, ISettingsStorage settingsStorage)
        {
            _environmentManager = environmentManager;
            _settingsStorage = settingsStorage;
        }

        public void CreateConfiguration(string name, SqlDatabaseType databaseType, string connectionString)
        {
            ConfigurationTools.CreateConfiguration(name, databaseType, connectionString, true);

            var startupConfig = new StartupConfig() { DatabaseType = databaseType, ConnectionString = connectionString };

            _settingsStorage.Get<AppConfig>().Environments.Add(startupConfig);
            _settingsStorage.Save();

            _environmentManager.AddWorkEnvironment(startupConfig);
        }




        public void AddConfiguration(SqlDatabaseType databaseType, string connectionString)
        {

            var startupConfig = new StartupConfig() { DatabaseType = databaseType, ConnectionString = connectionString };

            _settingsStorage.Get<AppConfig>().Environments.Add(startupConfig);
            _settingsStorage.Save();

            _environmentManager.AddWorkEnvironment(startupConfig);
        }


        public void BuildConfiguration(string name)
        {
            var env = _environmentManager.GetEnvironment(name);
            if (env is WorkEnvironment workEnvironment)
            {
                ConfigurationTools.BuildConfiguration(workEnvironment.Configuration, workEnvironment.DataContextManager.SqlCompiler,
                    workEnvironment.DataContextManager.GetContext());

                

            }
        }


    }
}
