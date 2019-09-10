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


        public PlatformAssemblyLoadContext PlatformAssemblyLoadContext { get; private set; }

        public PlatformClient(ILogger<PlatformClient> logger, IClient client, IServiceProvider serviceProvider)
        {
            _client = client;
            _logger = logger;

            _serviceProvider = serviceProvider;

        }

        public Assembly LoadMainAssembly()
        {
            var assemblyName = $"{ _client.Database}{Enum.GetName(typeof(CompilationMode), CompilationMode.Client)}";

            return PlatformAssemblyLoadContext.LoadFromAssemblyName(new AssemblyName(assemblyName));
        }


        public void Connect(DatabaseConnectionSettings connectionSettings)
        {
            _client.Connect(NetworkUtility.CreateIPEndPoint(connectionSettings.Address));
            _client.Use(connectionSettings.Database);

            _connectionSettings = connectionSettings;
            

            
        }


        public void Login(string name, string password)
        {
            _client.Authentication(new UserPasswordAuthenticationToken(name, password));


            PlatformAssemblyLoadContext =  _serviceProvider.GetRequiredService<PlatformAssemblyLoadContext>();
        }

        
    }
}
