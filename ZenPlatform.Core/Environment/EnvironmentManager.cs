using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Core.Logging;
using ZenPlatform.Core.DI;
using Microsoft.Extensions.DependencyInjection;
using ZenPlatform.Core.Contracts.Environment;
using ZenPlatform.Core.Environment.Contracts;
using ZenPlatform.Core.Settings;

namespace ZenPlatform.Core.Environment
{
    /*
     * Концептуально:
     *     У нас есть сервер приложений
     *     На сервере приложений поднят IAdminEnvirnoment, который следует переименовать в IServerAppEnvironment
     *     
     */

    /// <summary>
    /// Менеджер сред для платформы.
    ///
    /// <br /> Возжможно, потом, топологически каждая отдельная среда - это отдельный процесс
    ///  </summary>
    public class EnvironmentManager : IPlatformEnvironmentManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly List<IEnvironment> environments = new List<IEnvironment>();
        private readonly ILogger _logger;

        public EnvironmentManager(ISettingsStorage configStorage, IServiceProvider serviceProvider,
            ILogger<EnvironmentManager> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

            Initialize(configStorage.Get<AppConfig>().Environments);
        }

        private void Initialize(List<StartupConfig> list)
        {
            list.ForEach(c => environments.Add(CreatePlatformEnvironment<IWorkEnvironment>(c)));
        }

        public void AddWorkEnvironment(IStartupConfig config)
        {
            environments.Add(CreatePlatformEnvironment<IWorkEnvironment>(config));
        }

        protected IPlatformEnvironment CreatePlatformEnvironment<T>(IStartupConfig config)
            where T : IPlatformEnvironment
        {
            try
            {
                _logger.Info("Creating environment, connection string: {0}", config.ConnectionString);
                var scope = _serviceProvider.CreateScope();

                var env = scope.ServiceProvider.GetRequiredService<T>();
                env.Initialize(config);
                return env;
            }
            catch (Exception ex)
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