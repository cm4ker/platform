using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Authentication;
using Aquila.Core.Contracts.Network;
using Aquila.Core.Tools;
using Aquila.Logging;
using Aquila.Serializer;

namespace Aquila.Core.Network
{
    /// <summary>
    /// Имплементация клиента, который выполняет уже функции платформы поверх транспортного уровня
    ///
    /// <br />
    /// Внимание на данный момент клиент не является потокобезопасным 
    /// </summary>
    public class Client : IConnectionObserver<IConnectionContext>, IPlatformClient
    {
        private ConcurrentDictionary<Guid, Action<INetworkMessage>> _resultCallbacks;
        private readonly ILogger _logger;
        private readonly ITransportClientFactory _tcFactory;
        private ClientConnection _connection;
        private IDisposable _unsubscriber;
        private int _timeout = 1000000;

        //TODO: Сделать клиента потокобезопасным

        /// <summary>
        /// Создать клиента
        /// </summary>
        /// <param name="logger">Логгер</param>
        /// <param name="tcFactory">Фабрика клиентских подключений</param>
        public Client(ILogger<Client> logger, ITransportClientFactory tcFactory)
        {
            _logger = logger;
            _tcFactory = tcFactory;
            _resultCallbacks = new ConcurrentDictionary<Guid, Action<INetworkMessage>>();
        }

        /// <summary>
        /// Текущая база данных
        /// </summary>
        public string Database { get; private set; }

        /// <summary>
        /// База данных используется
        /// </summary>
        public bool IsUse => !string.IsNullOrEmpty(Database);

        /// <summary>
        /// Аутентификация пройдера
        /// </summary>
        public bool IsAuthenticated { get; private set; }

        /// <summary>
        /// Информация о соединении
        /// </summary>
        public IConnectionInfo Info => throw new NotImplementedException();


        /// <summary>
        /// Соединен
        /// </summary>
        public bool IsConnected { get; private set; }


        /// <summary>
        /// Подключиться
        /// </summary>
        /// <param name="endPoint">Адрес подключения</param>
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

                IsConnected = true;
            }
            catch (SocketException socketException)
            {
                IsConnected = false;

                _logger.Error(socketException, "Connection error: ");

                throw;
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

        public bool Authenticate(IAuthenticationToken token)
        {
            var req = new RequestAuthenticationNetworkMessage(token);

            _logger.Debug("Try send RequestAuthenticationNetworkMessage");

            var wait = RequestAsync(req, msg =>
            {
                switch (msg)
                {
                    case ResponceAuthenticationNetworkMessage res:
                        IsAuthenticated = true;

                        break;
                    case ErrorNetworkMessage res:
                        IsAuthenticated = false;

                        break;
                }
            });
            if (!wait.WaitOne(_timeout)) throw new TimeoutException();
            return IsAuthenticated;
        }


        public Stream InvokeAsStream(Route route, params object[] args)
        {
            if (!IsConnected && !IsAuthenticated)
                throw new NotSupportedException("Client is not connected or not authenticated.");

            var message = new StartInvokeStreamNetworkMessage(route, args);
            var stream = new DataStream(message.Id, _connection);

            _connection.Channel.Send(message);

            return stream;
        }

        public bool Use(string name)
        {
            if (!IsConnected) throw new NotSupportedException("Client is disconnected.");
            if (IsUse) return IsUse;

            var req = new RequestEnvironmentUseNetworkMessage(name);
            var wait = RequestAsync(req, msg =>
            {
                switch (msg)
                {
                    case ResponceEnvironmentUseNetworkMessage res:
                        Database = res.Name;
                        break;
                    case ErrorNetworkMessage res:
                        break;
                }
            });

            if (!wait.WaitOne(_timeout)) throw new TimeoutException();
            return IsUse;
        }

        public TResponse Invoke<TResponse>(Route route, params object[] args)
        {
            if (!IsConnected && !IsAuthenticated)
                throw new NotSupportedException("Client is not connected or not authenticated.");
            if (!IsUse) throw new NotSupportedException("First need to choose a database. Need call method Use.");

            TResponse responce = default;
            Exception exception = null;

            PlatformSerializer serializer = new PlatformSerializer();

            var byteArgs = serializer.Serialize(args);

            var message = new RequestInvokeUnaryByteArgsNetworkMessage(route, byteArgs);

            AutoResetEvent restEvent = new AutoResetEvent(false);

            var wait = RequestAsync(message, (msg) =>
            {
                switch (msg)
                {
                    case ErrorNetworkMessage error:
                        exception = error.Exception;
                        break;
                    case ResponceInvokeUnaryByteArgsNetworkMessage res:
                        try
                        {
                            responce = (TResponse)serializer.Deserialize(res.Result, true);
                        }
                        catch (Exception ex)
                        {
                            exception = ex;
                        }

                        break;
                }
            });


            if (!wait.WaitOne(_timeout)) throw new TimeoutException();
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
            if (IsConnected)
            {
                _logger.Info("Close connection.");

                _unsubscriber?.Dispose();
                IsConnected = false;
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