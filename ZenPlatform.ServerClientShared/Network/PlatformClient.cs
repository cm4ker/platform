using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Core.Logging;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Assemlies;
using ZenPlatform.Core.Settings;
using ZenPlatform.Core.DI;
using System.IO;
using System.Reflection;
using ZenPlatform.Core.Tools;
using ZenPlatform.QueryBuilder;
using Microsoft.Extensions.DependencyInjection;
using ZenPlatform.Compiler;

namespace ZenPlatform.Core.Network
{
    public class PlatformClient
    {
        private readonly IClient _client;
        private readonly ILogger _logger;
        private DatabaseConnectionSettings _connectionSettings;
        private readonly IServiceProvider _serviceProvider;


        private PlatformAssemblyLoadContext _platformAssemblyLoadContext;

        public PlatformClient(ILogger<PlatformClient> logger, IClient client, IServiceProvider serviceProvider)
        {
            _client = client;
            _logger = logger;

            _serviceProvider = serviceProvider;
        }

        public Assembly LoadMainAssembly()
        {
            var assemblyName = $"{_client.Database}{Enum.GetName(typeof(CompilationMode), CompilationMode.Client)}";

            return _platformAssemblyLoadContext.LoadFromAssemblyName(new AssemblyName(assemblyName));
        }


        public void Connect(DatabaseConnectionSettings connectionSettings)
        {
            _client.Connect(NetworkUtility.CreateIPEndPoint(connectionSettings.Address));

            _logger.Info("Try use DB..");
            _client.Use(connectionSettings.Database);
            _logger.Info("Success!");

            _connectionSettings = connectionSettings;
        }


        public void Login(string name, string password)
        {
            _logger.Info("Try login");
            if (_client.Authentication(new UserPasswordAuthenticationToken(name, password)))
            {
                _logger.Info("Success");
            }
            else
            {
                _logger.Info("Failure");
            }


            _platformAssemblyLoadContext = _serviceProvider.GetRequiredService<PlatformAssemblyLoadContext>();
        }
    }
}