using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Core.Logging;
using ZenPlatform.Core.Authentication;

namespace ZenPlatform.Core.Network
{
    public class DatabaseConnectionSettings
    {
        public string Address;
        public string Database;
    }
    public class PlatformClient
    {
        private readonly Client _client;
        private readonly ILogger _logger;

        public PlatformClient(ILogger<PlatformClient> logger, Client client)
        {
            _client = client;
            _logger = logger;
        }

        public void AddDatabase(DatabaseConnectionSettings connectionSettings)
        {

        }

        public bool ChackConnection(DatabaseConnectionSettings connectionSettings)
        {
            return true;
        }

        public void Connect(DatabaseConnectionSettings connectionSettings)
        {
            _client.Connect(NetworkUtility.CreateIPEndPoint(connectionSettings.Address));
            _client.Use(connectionSettings.Database);
        }


        public void Login(string name, string password)
        {
            _client.Authentication(new UserPasswordAuthenticationToken(name, password));

        }
    }
}
