using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Core.Settings;
using Aquila.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Aquila.Core.Instance
{
    /// <summary>
    /// Instance manager for host
    /// </summary>
    public class AqInstanceManager : IAqInstanceManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly List<AqInstance> environments = new List<AqInstance>();
        private readonly ILogger _logger;

        public AqInstanceManager(ISettingsStorage configStorage, IServiceProvider serviceProvider,
            ILogger<AqInstanceManager> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

            Initialize(configStorage.Get<AppConfig>().Environments);
        }

        private void Initialize(List<StartupConfig> list)
        {
            list.ForEach(AddInstance);
        }

        public void AddInstance(StartupConfig config)
        {
            var instance = CreatePlatformEnvironment<AqInstance>(config);

            if (instance != null)
                environments.Add(instance);
        }

        protected AqInstance CreatePlatformEnvironment<T>(StartupConfig config)
            where T : AqInstance
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

        public AqInstance GetInstance(string name)
        {
            return environments.FirstOrDefault(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<AqInstance> GetInstances()
        {
            return environments.AsReadOnly();
        }
    }
}