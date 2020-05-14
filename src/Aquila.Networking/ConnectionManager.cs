using System;
using System.Collections.Generic;
using System.Text;
using Aquila.Core.Tools;
using Aquila.Core.Logging;

namespace Aquila.Core.Network
{
    public class ConnectionManager : IConnectionManager
    {
        private readonly IList<Connection> _connections;
        private readonly ILogger _logger;

        public ConnectionManager(ILogger<ConnectionManager> logger)
        {
            _logger = logger;
        }

        public void AddConnection(Connection connection)
        {
            _connections?.Add(connection);
        }
    }
}