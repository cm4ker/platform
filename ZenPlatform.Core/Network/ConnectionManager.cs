using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Core.Tools;
using ZenPlatform.Core.Logging;

namespace ZenPlatform.Core.Network
{
    public class ConnectionManager : IConnectionManager
    {
        private readonly IList<TCPServerConnection> _connections = new RemovingList<TCPServerConnection>();
        private readonly ILogger _logger;

        public ConnectionManager(ILogger<ConnectionManager> logger)
        {
            _logger = logger;
        }

        public void AddConnection(TCPServerConnection connection)
        {
            _connections.Add(connection);
        }
    }
}
