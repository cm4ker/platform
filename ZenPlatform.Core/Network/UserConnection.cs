using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using ZenPlatform.ServerClientShared.Network;
using ZenPlatform.ServerClientShared.DI;
using ZenPlatform.Core.Environment;
using Newtonsoft.Json;
using ZenPlatform.Core.Logging;
using ZenPlatform.ServerClientShared.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace ZenPlatform.Core.Network
{
    public class UserConnection<T> : IDisposable,  IConnection<T> where T: IMessageHandler
    {
        private IChannel _channel;
        private TcpClient _client;
        private IDisposable _remover;
        
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public ConnectionInfo Info => throw new NotImplementedException();

        public UserConnection(ILogger<UserConnection<T>> logger, IServiceProvider serviceProvider, IChannel channel)
        {
            _channel = channel;
            _logger = logger;
            _serviceProvider = serviceProvider;
            
        }

        public void Open(TcpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            if (!client.Connected) throw new InvalidOperationException("Client must be connected.");

            _channel.OnError += (ex) =>
            {
                _logger.Info("Client '{0}' disconnected: '{1}'", client.Client.RemoteEndPoint, ex.Message);
                Dispose();
            };
            _channel.SetHandler(_serviceProvider.GetRequiredService<T>());
            _channel.Start(this);
            
        }

        public Stream GetStream()
        {
            return _client.GetStream();
        }

        public void Close()
        {
            if (_channel != null)
            {
                _channel.Stop();
            }
            
            if (_client != null)
            {
                _client.Dispose();
                
            }
            if (_remover != null)
                _remover.Dispose();
        }


        public void Dispose()
        {
            Close();
        }

        public void SetRemover(IDisposable remover)
        {
            if (_remover == null)
                _remover = remover;
            else remover.Dispose();
        }
    }
}
