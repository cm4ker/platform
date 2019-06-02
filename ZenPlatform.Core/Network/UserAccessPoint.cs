using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ZenPlatform.Core.Logging;
using ZenPlatform.ServerClientShared;
using ZenPlatform.ServerClientShared.DI;
using ZenPlatform.ServerClientShared.Logging;
using ZenPlatform.ServerClientShared.Network;
using ZenPlatform.ServerClientShared.Tools;

namespace ZenPlatform.Core.Network
{
    public class UserAccessPoint : IAccessPoint
    {
        private readonly ILogger _logger;
        private TcpListener _listener;
        private bool _running;
        private Thread _thread;
        private readonly List<IConnection> _connections;
        private readonly IDependencyResolver _dependencyResolver;
        private readonly AccessPointConfig _config;

        public UserAccessPoint(ILogger<UserAccessPoint> logger, IDependencyResolver dependencyResolver, IConfig<AccessPointConfig> config)
        {
            _logger = logger;
            _dependencyResolver = dependencyResolver;
            _connections = new List<IConnection>();
            _config = config.Value;
        }


        public void Start()
        {
            try
            {
                _logger.Info("Started listening: {0}", _config.Address);

                _listener = new TcpListener(NetworkUtility.CreateIPEndPoint(_config.Address));
                _listener.Start();
                

                _running = true;
                _thread = new Thread(ThreadListen);
                _thread.Start();
            } catch (Exception ex)
            {
                _logger.Error(ex, "Error start accsess point.");
            }

        }

        public void Stop()
        {
            if (_running)
            {
                _running = false;
                _thread.Interrupt();
                _listener.Stop();
                _logger.Info("Stoped listening");
            }
        }

        private void AddConnection(TcpClient client)
        {
            var connection = _dependencyResolver.Resolve<IConnection<IUserMessageHandler>>();
            

            _connections.Add(connection);

            connection.SetUnsubscriber(new ListUnsubscriber<IConnection>(_connections, connection));
            connection.Open(client);

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
                        _logger.Debug(ex, "Listen error: ");
                        Stop();
                    }
                }

                try
                {
                    if (client != null)
                    {
                        /*
                        var connection = _dependencyResolver.Resolve<IConnection<IUserMessageHandler>>();
                        
                        connection.Open(client);

                        _connections.Add(connection);

                        connection.Channel.OnError += (ex) =>
                        {


                            _logger.Info("Client '{0}' disconnected: '{1}'", client.Client.RemoteEndPoint, ex.Message);
                            connection.Close();
                            _connections.Remove(connection);
                        };
                        */
                        AddConnection(client);
                    }
                }
                catch(Exception)
                {

                }
            }
            
            
        }
    }
}
