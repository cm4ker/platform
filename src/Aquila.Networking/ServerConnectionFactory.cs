﻿using System;
using Microsoft.Extensions.DependencyInjection;
using Aquila.Core.Contracts.Instance;
using Aquila.Core.Network.Contracts;
using Aquila.Logging;

namespace Aquila.Core.Network
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
            IPlatformInstanceManager instanceManager)
        {
            var connection = new ServerConnection(
                _serviceProvider.GetRequiredService<ILogger<ServerConnection>>(),
                _serviceProvider.GetRequiredService<IChannelFactory>(),
                tcpClient,
                instanceManager
            );

            return connection;
        }

        public virtual Connection CreateConnection(ITransportClient tcpClient)
        {
            var connection = CreateConnection(
                tcpClient,
                _serviceProvider.GetRequiredService<IPlatformInstanceManager>()
            );

            return connection;
        }
    }
}