using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using ZenPlatform.Core.Logging;

namespace ZenPlatform.Core.Network
{
    public class ClientConnection : Connection
    {
        private Action<IChannel, INetworkMessage> _handler;

        public ClientConnection(ILogger logger, ITransportClient client, IChannelFactory channelFactory)
            : base(logger, client, channelFactory)
        {
        }

        public override void OnNext(INetworkMessage value)
        {
            foreach (var observer in _connectionObservers.ToArray())
                if (_connectionObservers.Contains(observer) && observer.CanObserve(value.GetType()))
                {
                    observer.OnNext(new ClientConnectionContext() {Connection = this}, value);
                    ;
                }
        }
    }
}