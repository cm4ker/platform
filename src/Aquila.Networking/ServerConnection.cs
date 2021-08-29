using System;
using Aquila.Core.Network.States;
using Aquila.Core.Contracts.Instance;
using Aquila.Logging;

namespace Aquila.Core.Network
{
    public class ServerConnection : Connection
    {
        private readonly ILogger<ServerConnection> _logger;
        private IDisposable _remover;

        //public IState State { get; set; }
        //public IEnvironmentManager EnvironmentManager { get; }
        //public IEnvironment Environment { get; set; }
        //public ISession Session { get; set; }

        private ServerConnectionContext _connectionContext;

        public ServerConnection(ILogger<ServerConnection> logger, IChannelFactory channelFactory,
            ITransportClient tcpClient, IPlatformInstanceManager instanceManager)
            : base(logger, tcpClient, channelFactory)
        {
            _logger = logger;
            _connectionContext = new ServerConnectionContext()
            {
                Connection = this
            };

            var state = new EnvironmentManagerObserver(instanceManager);
            state.Subscribe(this);
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