using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Contracts;
using ZenPlatform.Core.Logging;
using ZenPlatform.Core.Tools;

namespace ZenPlatform.Core.Network
{
    public class Client : IConnectionObserver<IConnectionContext>, IClient
    {
        private ConcurrentDictionary<Guid, Action<INetworkMessage>> _resultCallbacks;
        private readonly ILogger _logger;
        private readonly ITransportClientFactory _tcFactory;
        private ClientConnection _connection;
        private IDisposable _unsubscriber;

        public string Database { get; private set; }
        public bool IsUse { get; private set; }
        public bool Authenticated { get; private set; }

        public ConnectionInfo Info => throw new NotImplementedException();

        public bool Connected { get; private set; }

        public Client(ILogger<Client> logger, ITransportClientFactory tcFactory)

        {
            _logger = logger;
            _tcFactory = tcFactory;
            _resultCallbacks = new ConcurrentDictionary<Guid, Action<INetworkMessage>>();
        }

        public void Connect(IPEndPoint endPoint)
        {
            _logger.Info("Connect to {0}", endPoint.Address.ToString());
            try
            {
                var transport = _tcFactory.Create(endPoint);

                _connection = new ClientConnection(new SimpleConsoleLogger<ClientConnection>(),
                    transport, new ClientChannelFactory());
                _unsubscriber = _connection.Subscribe(this);
                _connection.Open();

                Connected = true;
            }
            catch (SocketException socketException)
            {
                Connected = false;

                _logger.Error(socketException, "Connection error: ");

                throw socketException;
            }
        }

        private WaitHandle RequestAsync(INetworkMessage message, Action<INetworkMessage> CallBack)
        {
            AutoResetEvent restEvent = new AutoResetEvent(false);
            _resultCallbacks.TryAdd(message.Id, (m) =>
            {
                CallBack(m);
                _resultCallbacks.TryRemove(message.Id, out _);
                restEvent.Set();
            });
            _connection.Channel.Send(message);

            return restEvent;
        }

        public bool Authentication(IAuthenticationToken token)
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
            return Authenticated;
        }


        public Stream InvokeStream(Route route, params object[] args)
        {
            if (!Connected && !Authenticated)
                throw new NotSupportedException("Client is not connected or not authenticated.");

            var message = new StartInvokeStreamNetworkMessage(route, args);
            var stream = new DataStream(message.Id, _connection);

            _connection.Channel.Send(message);

            return stream;
        }

        public bool Use(string name)
        {
            if (!Connected) throw new NotSupportedException("Client is disconnected.");
            if (IsUse) return IsUse; //todo

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
                        IsUse = false;
                        break;
                }
            });
            wait.WaitOne();
            return IsUse;
        }

        public TResponce Invoke<TResponce>(Route route, params object[] args)
        {
            if (!Connected && !Authenticated)
                throw new NotSupportedException("Client is not connected or not authenticated.");
            if (!IsUse) throw new NotSupportedException("First need to choose a database. Need call method Use.");

            TResponce responce = default;
            Exception exception = null;
            var message = new RequestInvokeUnaryNetworkMessage(route, args);

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
                            responce = (TResponce) res.Result;
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

        public async Task<TResponce> InvokeAsync<TResponce>(Route route, params object[] args)
        {
            return await Task.Factory.StartNew(() => Invoke<TResponce>(route, args)
                , TaskCreationOptions.LongRunning);
        }


        public void Close()
        {
            if (Connected)
            {
                _logger.Info("Close connection.");

                _unsubscriber?.Dispose();
                Connected = false;
                _connection?.Close();
            }
        }

        public T GetService<T>()
        {
            var factory = new NetworkProxyFactory();

            return factory.Create<T>(_connection);
        }


        public void OnCompleted(IConnectionContext sender)
        {
            Close();
        }

        public void OnError(IConnectionContext sender, Exception error)
        {
            Close();
        }

        public void OnNext(IConnectionContext context, INetworkMessage value)
        {
            if (value is ErrorNetworkMessage error)
                _logger.Debug("Error message: ", error.Exception);

            if (_resultCallbacks.ContainsKey(value.RequestId))
            {
                _resultCallbacks[value.RequestId](value);
            }
        }

        public bool CanObserve(Type type)
        {
            return true;
        }
    }
}