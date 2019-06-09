using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ZenPlatform.ServerClientShared.DI;
using ZenPlatform.ServerClientShared.Logging;
using ZenPlatform.ServerClientShared.Network;
using Microsoft.Extensions.DependencyInjection;
using ZenPlatform.Core.Network.States;

namespace ZenPlatform.Core.Network
{

    public class UserListener : Listener, IUserListener
    {

        public IServiceProvider _serviceProvider;
        public UserListener(IServiceProvider serviceProvider, ILogger<UserListener> logger, IConnectionManager connectionManager) 
            :base(logger, connectionManager)
        {
            _serviceProvider = serviceProvider;
        }
        protected override IConnection OpenConnection()
        {
            return _serviceProvider.GetRequiredService<IUserConnection>();
        }
    }
    public abstract class Listener : IListener
    {
        private readonly ILogger _logger;
        private readonly IConnectionManager _connectionManager;
        private TcpListener _listener;
        private bool _running;
        private Thread _thread;

        public Listener(ILogger logger, IConnectionManager connectionManager)
        {
            _logger = logger;
            _connectionManager = connectionManager;
        }

        public void Start(IPEndPoint endPoint)
        {
            try
            {
                _logger.Info("Started listening: {0}", endPoint.Address);

                _listener = new TcpListener(endPoint);
                _listener.Start();


                _running = true;
                _thread = new Thread(ThreadListen);
                _thread.Start();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error starting listener.");
            }

        }

        public void Stop()
        {
            if (_running)
            {
                _running = false;
                _listener.Stop();
                _logger.Info("Stoped listening");
            }
        }



        private void ThreadListen()
        {

            while (_running)
            {
                TcpClient client = null;
                try
                {
                    client = _listener.AcceptTcpClient();
                    _logger.Info("Client connected: '{0}'", client.Client.RemoteEndPoint.ToString());



                }

                catch (Exception ex)
                {
                    if (_running)
                    {
                        _logger.Debug(ex, "Accept client error: ");
                        Stop();
                    }
                }

                try
                {
                    if (client != null)
                    {
                        var connection = OpenConnection();
                        _connectionManager.AddConnection(connection);
                        connection.Open(client);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Debug(ex, "Can't open client connection: ");
                }
            }


        }

        protected abstract IConnection OpenConnection();

    }
}
