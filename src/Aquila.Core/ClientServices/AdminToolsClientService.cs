using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Aquila.Core.ClientServices;
using Aquila.Core.Configuration;
using Aquila.Core.Contracts.Environment;
using Aquila.Core.Environment;
using Aquila.Core.Network;
using Aquila.Core.Settings;
using Aquila.QueryBuilder;

namespace Aquila.Core.Tools
{
    public class AdminToolsClientService : IAdminToolsClientService
    {
        private IPlatformEnvironmentManager _environmentManager;
        private ISettingsStorage _settingsStorage;
        private IConfigurationManager _configurationManager;

        public AdminToolsClientService(IPlatformEnvironmentManager environmentManager, ISettingsStorage settingsStorage,
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