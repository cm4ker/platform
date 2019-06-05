using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.ServerClientShared.Network;
using ZenPlatform.ServerClientShared.Tools;
using ZenPlatform.ServerClientShared.Logging;

namespace ZenPlatform.Core.Network
{
    public class ConnectionManager : IConnectionManager
    {
        private readonly IList<IConnection> _connections = new RemovingList<IConnection>();
        private readonly ILogger _logger;

        public ConnectionManager(ILogger<ConnectionManager> logger)
        {
            _logger = logger;
        }

        public void AddConnection(IConnection connection)
        {
            _connections.Add(connection);
        }
    }
}
