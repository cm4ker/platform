
using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Core.Logging;
using ZenPlatform.ServerClientShared.DI;
using ZenPlatform.ServerClientShared.Logging;

namespace ZenPlatform.Core.Environment
{
    public class EnvironmentManager : IEnvironmentManager
    {
        private readonly IDependencyResolver _dependencyResolver;
        private readonly List<IEnvironment> environments = new List<IEnvironment>();
        private readonly ILogger _logger;

        public EnvironmentManager(IConfig<List<StartupConfig>> config, IDependencyResolver dependencyResolver, ILogger<EnvironmentManager> logger)
        {
            _dependencyResolver = dependencyResolver;
            _logger = logger;

            config.Value.ForEach(c => CreateEnvironment(c));
        }

        public void CreateEnvironment(StartupConfig config)
        {
            _logger.Info("Creating environment, connection string: {0}", config.ConnectionString);
            try
            {
                using (var scope = _dependencyResolver.BeginScope())
                {
                    var env = scope.Resolver.Resolve<IEnvironment>();
                    env.Initialize(config);
                    environments.Add(env);
                }
            } catch (Exception ex)
            {
                _logger.Error(ex, "Create environment error, connection string: {0}", config.ConnectionString);
            }
        }

        public IEnvironment GetEnvironment(string name)
        {
            return environments.First(m => m.Configuration.ProjectName.Equals(name));
        }

        public List<string> GetEnvironmentList()
        {
            return environments.Select(e => e.Configuration.ProjectName).ToList();
        }
    }
}
