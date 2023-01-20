using System;
using System.Reflection;
using Aquila.Core.Assemlies;
using Aquila.Core.Authentication;
using Aquila.Core.Contracts.Network;
using Aquila.Core.Infrastructure.Settings;
using Aquila.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Aquila.Core.Network
{
    public interface IClientPlatformContext
    {
        Assembly LoadMainAssembly();

        Assembly MainAssembly { get; }

        //IPlatformClient Client { get; }
        void Connect(DatabaseConnectionSettings connectionSettings);
        void Login(string name, string password);
    }

    /// <summary>
    /// Контекст платформы
    /// <br />
    /// на этом уровне Клиентское соединение платформы оборачивается. Есть позможность работать со сборками (выгрузка загрузка)
    /// </summary>
    public class ClientPlatformContext : IClientPlatformContext
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
            throw new NotImplementedException();

            // var assemblyName = $"{_client.Database}{Enum.GetName(typeof(CompilationMode), CompilationMode.Client)}";
            //
            // MainAssembly = _platformAssemblyLoadContext.LoadFromAssemblyName(new AssemblyName(assemblyName));
            //
            // return MainAssembly;
        }

        public Assembly MainAssembly { get; private set; }


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