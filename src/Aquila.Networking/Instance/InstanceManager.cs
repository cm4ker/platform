using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Aquila.Core.Contracts.Instance;
using Aquila.Core.Settings;
using Aquila.Logging;

namespace Aquila.Core.Instance
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
        private readonly List<IPlatformInstance> environments = new List<IPlatformInstance>();
        private readonly ILogger _logger;

        public InstanceManager(ISettingsStorage configStorage, IServiceProvider serviceProvider,
            ILogger<InstanceManager> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

            Initialize(configStorage.Get<AppConfig>().Environments);
        }

        private void Initialize(List<IStartupConfig> list)
        {
            list.ForEach(AddInstance);
        }

        public void AddInstance(IStartupConfig config)
        {
            var instance = CreatePlatformEnvironment<IPlatformInstance>(config);

            if (instance != null)
                environments.Add(instance);
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

        public IPlatformInstance GetInstance(string name)
        {
            return environments.FirstOrDefault(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<IPlatformInstance> GetInstances()
        {
            return environments.AsReadOnly();
        }
    }
}