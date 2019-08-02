using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Network.States;
using ZenPlatform.Core.Logging;
using ZenPlatform.Core.Tools;
using ZenPlatform.Core.Sessions;
using ZenPlatform.Core.Authentication;

namespace ZenPlatform.Core.Network
{
    public class TCPServerConnection : TCPConnection, IRemovable
    {
        private IDisposable _remover;

        //public IState State { get; set; }
        //public IEnvironmentManager EnvironmentManager { get; }
        //public IEnvironment Environment { get; set; }
        //public ISession Session { get; set; }

        private ServerConnectionContext _connectionContext;

        public TCPServerConnection(ILogger<TCPServerConnection> logger, IChannelFactory channelFactory, 
            TcpClient tcpClient, IEnvironmentManager environmentManager) 
            : base(logger, tcpClient, channelFactory)
        {

            var state = new ConnectedState(environmentManager);
            state.Subscribe(this);
 
            _connectionContext = new ServerConnectionContext()
            {
                Connection = this
            };

            


        }

        public void SetRemover(IDisposable remover)
        {
            if (_remover == null)
                _remover = remover;
            else remover.Dispose();
        }

        public override void Close()
        {
            base.Close();
            _remover?.Dispose();
            _connectionContext.Session?.Close();
        }

        public override void OnNext(INetworkMessage value)
        {

            foreach (var observer in _connectionObservers.ToArray())
            if (_connectionObservers.Contains(observer) && observer.CanObserve(value.GetType()))
            {
                observer.OnNext(_connectionContext, value);
            }
           
        }

        public override void OnCompleted()
        {
            foreach (var observer in _connectionObservers.ToArray())
            if (_connectionObservers.Contains(observer))
            {
                observer.OnCompleted(_connectionContext);
            }
            base.OnCompleted();
        }

        public override void OnError(Exception error)
        {
            foreach (var observer in _connectionObservers.ToArray())
                if (_connectionObservers.Contains(observer))
                {
                    observer.OnError(_connectionContext, error);
                }


            base.OnError(error);
        }
    }


    
}
