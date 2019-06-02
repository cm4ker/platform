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
using ZenPlatform.ServerClientShared.Tools;

namespace ZenPlatform.Core.Network
{
    public class UserConnection<T> : IDisposable,  IConnection<T> where T: IMessageHandler
    {
        public IChannel Channel { get; private set; }
        private TcpClient _client;
        private IDisposable _unsubscriber;
        
        private readonly ILogger _logger;
        private readonly IDependencyResolver _dependencyResolver;

        public UserConnection(ILogger<UserConnection<T>> logger, IDependencyResolver dependencyResolver)
        {

            _logger = logger;
            _dependencyResolver = dependencyResolver;
            
        }

        public void Open(TcpClient client)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (!client.Connected) throw new InvalidOperationException("Client must be connected.");

            _client = client;
            Channel = _dependencyResolver.Resolve<IChannel>();
            Channel.Start(client.GetStream(), _dependencyResolver.Resolve<T>());

            Channel.OnError += (ex) =>
            {
                _logger.Info("Client '{0}' disconnected: '{1}'", client.Client.RemoteEndPoint, ex.Message);
                Dispose();
            };
        }

        public void Close()
        {
            if (Channel != null)
            {
                Channel.Stop();
            }
            
            if (_client != null)
            {
                _client.Dispose();
                
            }
        }


        public void Dispose()
        {
            Close();
            if (_unsubscriber!=null)
                _unsubscriber.Dispose();
        }

        public void SetUnsubscriber(IDisposable unsubscriber)
        {
            _unsubscriber = unsubscriber;
        }
    }
}
