using System;
using Microsoft.Extensions.DependencyInjection;
using ZenPlatform.Core.Logging;
using ZenPlatform.Core.Environment;
using System.Net.Sockets;
using ZenPlatform.Core.Network.Contracts;

namespace ZenPlatform.Core.Network
{
    /// <summary>
    /// Создает серверные соединения
    /// </summary>
    public class ServerConnectionFactory : IConnectionFactory
    {
        protected readonly IServiceProvider _serviceProvider;

        public ServerConnectionFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected Connection CreateConnection(ITransportClient tcpClient,
            IPlatformEnvironmentManager environmentManager)
        {
            var connection = new ServerConnection(
                _serviceProvider.GetRequiredService<ILogger<ServerConnection>>(),
                _serviceProvider.GetRequiredService<IChannelFactory>(),
                tcpClient,
                environmentManager
            );

            return connection;
        }

        public virtual Connection CreateConnection(ITransportClient tcpClient)
        {
            var connection = CreateConnection(
                tcpClient,
                _serviceProvider.GetRequiredService<IPlatformEnvironmentManager>()
            );

            return connection;
        }
    }
}