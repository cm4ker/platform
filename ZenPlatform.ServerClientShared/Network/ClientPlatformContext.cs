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
using ZenPlatform.Core.Network.Contracts;

namespace ZenPlatform.Core.Network
{
    /// <summary>
    /// Контекст платформы
    /// <br />
    /// на этом уровне Клиентское соединение платформы оборачивается. Есть позможность работать со сборками (выгрузка загрузка)
    /// </summary>
    public class ClientPlatformContext
    {
        private readonly IPlatformClient _client;
        private readonly ILogger _logger;
        private DatabaseConnectionSettings _connectionSettings;
        private readonly IServiceProvider _serviceProvider;

        private PlatformAssemblyLoadContext _platformAssemblyLoadContext;

        public ClientPlatformContext(ILogger<ClientPlatformContext> logger, IPlatformClient client,
            IServiceProvider serviceProvider)
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


        public IPlatformClient Client => _client;

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
            if (_client.Authenticate(new UserPasswordAuthenticationToken(name, password)))
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