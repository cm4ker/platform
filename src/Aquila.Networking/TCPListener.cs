using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Aquila.Core.Contracts.Environment;
using Aquila.Core.Environment;
using Aquila.Core.Logging;

namespace Aquila.Core.Network
{
    public class TCPListener : IDatabaseNetworkListener
    {
        private readonly ILogger _logger;
        private readonly IConnectionManager _connectionManager;
        private TcpListener _listener;
        private bool _running;
        private Thread _thread;
        IServiceProvider _serviceProvider;

        public TCPListener(ILogger<TCPListener> logger, IServiceProvider serviceProvider, IConnectionManager connectionManager)
        {
            _logger = logger;
            _connectionManager = connectionManager;
            _serviceProvider = serviceProvider;
        }

        public void Start(IPEndPoint endPoint)
        {
            try
            {

                _logger.Info("Started listening: {0}:{1}", endPoint.Address, endPoint.Port);


                _listener = new TcpListener(endPoint ?? throw new ArgumentNullException(nameof(endPoint)));
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
                ITransportClient client = null;
                try
                {
                    client = new TCPTransportClient(_listener.AcceptTcpClient());
                    _logger.Info("Client connected: '{0}'", client.RemoteEndPoint);
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
                        var connection = CreateConnection(client);
                        _connectionManager.AddConnection(connection);
                        connection.Open();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Debug(ex, "Can't open client connection: ");
                }
            }
        }

        private Connection CreateConnection(ITransportClient client)
        {
            return new ServerConnection(
                _serviceProvider.GetRequiredService<ILogger<ServerConnection>>(),
                _serviceProvider.GetRequiredService<IChannelFactory>(),
                client,
                _serviceProvider.GetRequiredService<IPlatformEnvironmentManager>()
            );
        }

        public void Dispose()
        {
            Stop();
        }
    }
}