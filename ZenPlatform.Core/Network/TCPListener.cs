using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ZenPlatform.Core.Logging;

namespace ZenPlatform.Core.Network
{
    public class TCPListener : ITCPListener
    {
        private readonly ILogger _logger;
        private readonly IConnectionManager _connectionManager;
        private TcpListener _listener;
        private bool _running;
        private Thread _thread;
        private ServerConnectionFactory _connectionFactory;

        public TCPListener(ILogger<TCPListener> logger, IConnectionManager connectionManager)
        {
            _logger = logger;
            _connectionManager = connectionManager;
        }

        public void Start(IPEndPoint endPoint, ServerConnectionFactory connectionFactory)
        {
            try
            {
                _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));

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
                        var connection = _connectionFactory.CreateConnection(client);
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
    }
}