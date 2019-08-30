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
using ZenPlatform.Core.ClientServices;

namespace ZenPlatform.Core.Network
{
    
    public class PlatformClient
    {
        private readonly Client _client;
        private readonly ILogger _logger;
        private DatabaseConnectionSettings _connectionSettings;
        public ClientAssemblyManager AssemblyManager { get; private set; }




        public PlatformClient(ILogger<PlatformClient> logger, Client client)
        {
            _client = client;
            _logger = logger;
            
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

            AssemblyManager = new ClientAssemblyManager(_client.GetService<IAssemblyManagerClientService>(),

                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "ZenClient", _connectionSettings.Database, "AssemblyCashe"));


            AssemblyManager.UpdateAssemblyes();

            AssemblyManager.LoadAssemblyes();

        }

        
    }
}
