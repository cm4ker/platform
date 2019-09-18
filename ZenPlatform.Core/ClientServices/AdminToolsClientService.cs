using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ZenPlatform.Core.ClientServices;
using ZenPlatform.Core.Configuration;
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
        private IConfigurationManager _configurationManager;

        public AdminToolsClientService(IEnvironmentManager environmentManager, ISettingsStorage settingsStorage,
            IConfigurationManager configurationManager)
        {
            _environmentManager = environmentManager;
            _settingsStorage = settingsStorage;
            _configurationManager = configurationManager;
        }

        public void CreateConfiguration(string name, SqlDatabaseType databaseType, string connectionString)
        {
            _configurationManager.CreateConfiguration(name, databaseType, connectionString);

            var startupConfig = new StartupConfig() {DatabaseType = databaseType, ConnectionString = connectionString};

            _settingsStorage.Get<AppConfig>().Environments.Add(startupConfig);
            _settingsStorage.Save();

            _environmentManager.AddWorkEnvironment(startupConfig);
        }

        public void AddConfiguration(SqlDatabaseType databaseType, string connectionString)
        {
            var startupConfig = new StartupConfig() {DatabaseType = databaseType, ConnectionString = connectionString};

            _settingsStorage.Get<AppConfig>().Environments.Add(startupConfig);
            _settingsStorage.Save();

            _environmentManager.AddWorkEnvironment(startupConfig);
        }
    }
}