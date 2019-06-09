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
using ZenPlatform.ServerClientShared.Tools;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Sessions;
using ZenPlatform.Core.Network.States;

namespace ZenPlatform.Core.Network
{

    public interface IUserConnection : IConnection { };
    public interface IAdminConnection : IConnection { };
    public abstract class Connection : IDisposable,  IConnection, IObserver<INetworkMessage> 
    {
        private readonly IChannel _channel;
        private TcpClient _client;
        private IDisposable _remover;
        private IDisposable _unsubscriber;
        
        private readonly ILogger _logger;

        public bool Opened { get; private set; }

        public ConnectionInfo Info => throw new NotImplementedException();

        public Connection(ILogger logger,  IChannel channel)
        {
            _channel = channel;
            _logger = logger;
           


        }

        public void Open(TcpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            if (!client.Connected) throw new InvalidOperationException("Client must be connected.");


            _unsubscriber = _channel.Subscribe(this);

            _channel.Start(this);

            Opened = true;
        }

        public Stream GetStream()
        {
            return _client.GetStream();
        }

        public void Close()
        {
            if (Opened)
            {
                _remover?.Dispose();
                _unsubscriber?.Dispose();
                _channel?.Stop();
                if (_client?.Connected == true)
                {
                    _client.GetStream().Close();

                    _client.Close();
                }
                _client?.Dispose();
                Opened = false;
            }
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

        public virtual void OnCompleted()
        {
            _logger.Info("Client '{0}' disconnected.", _client.Client.RemoteEndPoint);
            Close();
        }

        public virtual void OnError(Exception error)
        {
            _logger.Info("Client '{0}' disconnected: '{1}'", _client.Client.RemoteEndPoint, error.Message);
            Close();
        }

        public abstract void OnNext(INetworkMessage value);

       
    }
}
