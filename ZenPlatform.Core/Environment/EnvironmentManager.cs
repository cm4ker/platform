
using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Core.Logging;
using ZenPlatform.Core.DI;
using Microsoft.Extensions.DependencyInjection;
using ZenPlatform.Core.Settings;

namespace ZenPlatform.Core.Environment
{
    public class EnvironmentManager : IEnvironmentManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly List<IEnvironment> environments = new List<IEnvironment>();
        private readonly ILogger _logger;

        public EnvironmentManager(ISettingsStorage configStorage, IServiceProvider serviceProvider, ILogger<EnvironmentManager> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

            Initialize(configStorage.Get<AppConfig>().Environments);
        }

        private void Initialize(List<StartupConfig> list)
        {
            list.ForEach(c => environments.Add(CreateEnvironment<IWorkEnvironment>(c)));

            environments.Add(CreateEnvironment<IAdminEnvironment>(new StartupConfig() { ConnectionString = ""}));

#if DEBUG
            environments.Add(CreateEnvironment<ITestEnvironment>(new StartupConfig() { ConnectionString = "" }));
#endif
        }

        public void AddWorkEnvironment(StartupConfig config)
        {
            environments.Add(CreateEnvironment<IWorkEnvironment>(config));
        }

        protected IEnvironment CreateEnvironment<T>(StartupConfig config) where T: IEnvironment
        {
            
            try
            {
                _logger.Info("Creating environment, connection string: {0}", config.ConnectionString);
                var scope = _serviceProvider.CreateScope();
                
                    var env = scope.ServiceProvider.GetRequiredService<T>();
                    env.Initialize(config);
                    return env;
                
            } catch (Exception ex)
            {
                _logger.Error(ex, "Create environment error, connection string: {0}", config.ConnectionString);
            }
            return null;
        }

        public IEnvironment GetEnvironment(string name)
        {
            return environments.First(m => m.Name.Equals(name));
        }

        public List<IEnvironment> GetEnvironmentList()
        {
            return environments;
        }

    }
}
