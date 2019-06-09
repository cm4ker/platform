
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ZenPlatform.Core.Authentication;
using ZenPlatform.ServerClientShared.Logging;

namespace ZenPlatform.ServerClientShared.Network
{
    public class Client: IDisposable, IConnection, IObserver<INetworkMessage>
    {
        private Channel _channel;
        private readonly IMessagePackager _packager;
        private ConcurrentDictionary<Guid, Action<INetworkMessage>> _resultCallbacks;
        private readonly ILogger _logger;
        private TcpClient _tcpClient;
        private IDisposable _remover;
        private IDisposable _unsubscriber;

        public string Database { get; private set; }
        public bool IsUse { get; private set; }
        public bool Authenticated { get; private set; }

        public ConnectionInfo Info => throw new NotImplementedException();

        public bool Opened { get; private set; }

        public Client( IMessagePackager packager, ILogger<Client> logger)
        {
            
            _packager = packager;
            _logger = logger;
            _resultCallbacks = new ConcurrentDictionary<Guid, Action<INetworkMessage>>();
            _channel = new Channel(_packager, new SimpleConsoleLogger<Channel>());

        }

        public void Open(TcpClient client)
        {
            
            try
            {
               
                _tcpClient = client;
                _unsubscriber = _channel.Subscribe(this);
                _channel.Start(this);
                Opened = true;
            }
            catch (SocketException socketException)
            {
                Opened = false;

                _logger.Error(socketException, "Connection error: ");

            }
        }

        public void Open(IPEndPoint endPoint)
        {
            _logger.Info("Connect to {0}", endPoint.Address.ToString());
            try
            {
                var tcpClient = new TcpClient();
                tcpClient.Connect(endPoint);
                Open(tcpClient);
            }
            catch (SocketException socketException)
            {
                Opened = false;

                _logger.Error(socketException, "Connection error: ");

                throw socketException;
            }

        }

        private WaitHandle RequestAsync(INetworkMessage message, Action<INetworkMessage> CallBack)
        {
            AutoResetEvent restEvent = new AutoResetEvent(false);
            _resultCallbacks.TryAdd(message.Id, (m) => { CallBack(m); _resultCallbacks.TryRemove(message.Id, out _); restEvent.Set(); }) ;
            _channel.Send(message);

            return restEvent;


        }

        public void Authentication(IAuthenticationToken token)
        {
            var req = new RequestAuthenticationNetworkMessage(token);
            var wait = RequestAsync(req, msg =>
            {
                switch (msg)
                {
                    case ResponceAuthenticationNetworkMessage res:
                        Authenticated = true;
                        break;
                    case ErrorNetworkMessage res:
                        Authenticated = false;
                        break;
                }
                
            });
            wait.WaitOne();
        }


        public async Task<TResponce> InvokeAsync<TResponce>(Route route, params object[] args)
        {
            return await InvokeAsync<object[], TResponce> (route, args);
        }

        public Stream InvokeStream(Route route, params object[] args)
        {
            if (!Opened && !Authenticated) throw new NotSupportedException("Client is not connected or not authenticated.");

            var message = new StartInvokeStreamNetworkMessage(route, args);
            var stream = new DataStream(message.Id, _channel);

            _channel.Send(message);

            return stream;
        }

        public void Use(string name)
        {
            if (!Opened) throw new NotSupportedException("Client is disconnected.");
            if (IsUse) return;

            var req = new RequestEnvironmentUseNetworkMessage(name);
            var wait = RequestAsync(req, msg =>
             {
                 switch (msg)
                 {
                     case ResponceEnvironmentUseNetworkMessage res:
                         IsUse = true;
                         Database = res.Name;
                         break;
                     case ErrorNetworkMessage res:

                         break;
                 }
             });
            wait.WaitOne();
            
        }

        public TResponce Invoke<TRequest, TResponce>(Route route, TRequest request)
        {
            if (!Opened && !Authenticated) throw new NotSupportedException("Client is not connected or not authenticated.");
            if (!IsUse) throw new NotSupportedException("First need to choose a database. Need call method Use.");

            TResponce responce = default;
            Exception exception = null;
            var message = new RequestInvokeUnaryNetworkMessage(route, request);

            AutoResetEvent restEvent = new AutoResetEvent(false);

            var wait = RequestAsync(message, (msg) =>
            {
                switch (msg)
                {
                    case ErrorNetworkMessage error:
                        exception = error.Exception;
                        break;
                    case ResponceInvokeUnaryNetworkMessage res:
                        try
                        {
                            responce = (TResponce)res.Result;
                        }
                        catch (Exception ex)
                        {
                            exception = ex;
                        }
                        break;
                }

            });

            wait.WaitOne();
            if (exception != null) throw exception;

            return responce;

        }

        public async Task<TResponce> InvokeAsync<TRequest, TResponce>(Route route, TRequest request)
        {

            return await Task.Factory.StartNew(() => Invoke<TRequest, TResponce>(route, request)
            , TaskCreationOptions.LongRunning);
        }


        public void Dispose()
        {
            Close();
        }

        

        public void SetRemover(IDisposable remover)
        {
            _remover = remover;
        }

        public void OnCompleted()
        {
            //throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            _logger.Error(error, "Channal error: ");
            Close();
        }

        public void OnNext(INetworkMessage value)
        {
            if (value is ErrorNetworkMessage error)
                _logger.Debug( "Error message: ", error.Exception);

            if (_resultCallbacks.ContainsKey(value.RequestId))
            {
                _resultCallbacks[value.RequestId](value);
            }
        }

        public void Close()
        {
            if (Opened)
            {
                _logger.Info("Close connection.");

                
                Opened = false;
                _channel.Stop();
                if (_tcpClient?.Connected == true)
                {
                    _tcpClient.GetStream().Close();

                    _tcpClient.Close();
                }
                _tcpClient?.Dispose();
                _remover?.Dispose();
                _unsubscriber?.Dispose();
            }
        }

        public Stream GetStream()
        {
            return _tcpClient?.GetStream();
        }
    }
}
