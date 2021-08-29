using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Aquila.Core.Contracts.Instance;
using Aquila.Core.Settings;
using Aquila.Logging;

namespace Aquila.Core.Environment
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
    public class InstanceManager : IPlatformInstanceManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly List<IInstance> environments = new List<IInstance>();
        private readonly ILogger _logger;

        public InstanceManager(ISettingsStorage configStorage, IServiceProvider serviceProvider,
            ILogger<InstanceManager> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

            Initialize(configStorage.Get<AppConfig>().Environments);
        }

        private void Initialize(List<StartupConfig> list)
        {
            list.ForEach(c => environments.Add(CreatePlatformEnvironment<IWorkInstance>(c)));
        }

        public void AddWorkInstance(IStartupConfig config)
        {
            environments.Add(CreatePlatformEnvironment<IWorkInstance>(config));
        }

        protected IPlatformInstance CreatePlatformEnvironment<T>(IStartupConfig config)
            where T : IPlatformInstance
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

        public IInstance GetInstance(string name)
        {
            return environments.First(m => m.Name.Equals(name));
        }

        public List<IInstance> GetInstanceList()
        {
            return environments;
        }
    }
}