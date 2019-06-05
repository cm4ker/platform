
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
    public class Client: IMessageHandler, IDisposable, IConnection
    {
        private Channel _channel;
        private TcpClient _tcpClient;
        private readonly IMessagePackager _packager;
        private ConcurrentDictionary<Guid, Action<INetworkMessage>> _resultCallbacks;
        private readonly ILogger _logger;

        public string Database { get; private set; }
        public bool IsUse { get; private set; }

        public bool Connected { get; private set; }
        public bool Authenticated { get; private set; }

        public ConnectionInfo Info => throw new NotImplementedException();

        public Client( IMessagePackager packager, ILogger<Client> logger)
        {
            _tcpClient = new TcpClient();
            _packager = packager;
            _logger = logger;
            _resultCallbacks = new ConcurrentDictionary<Guid, Action<INetworkMessage>>();

        }

        public void Connect(IPEndPoint endPoint)
        {

            
            try
            {
                _logger.Info("Connect to {0}", endPoint.Address.ToString());
                _tcpClient.Connect(endPoint);
                _channel = new Channel( _packager, new SimpleConsoleLogger<Channel>());
                _channel.SetHandler(this);
                _channel.Start( this);
                Connected = true;
            }
            catch (SocketException socketException)
            {
                Connected = false;

                _logger.Error(socketException, "Connection error: ");

                throw socketException;
            } 
        }

        public void Receive(object message, IChannel channel)
        {
            switch (message)
            {
                
                case ErrorNetworkMessage res:
                    _logger.Trace(res.Exception.Message);
                    break;
                    
                case INetworkMessage res:
                    if (_resultCallbacks.ContainsKey(res.RequestId))
                    {
                        _resultCallbacks[res.RequestId](res);
                    }
                    break;
            }
            
        }

        public void Authentication(IAuthenticationToken token)
        {
            var req = new RequestAuthenticationNetworkMessage(token);
            AutoResetEvent restEvent = new AutoResetEvent(false);
            
            _resultCallbacks.TryAdd(req.Id, msg =>
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
                _resultCallbacks.TryRemove(req.Id, out _);
                restEvent.Set();
            });
            _channel.Send(req);
            restEvent.WaitOne();
        }



        public async Task<TResponce> InvokeAsync<TResponce>(Route route, params object[] args)
        {
            return await InvokeAsync<object[], TResponce> (route, args);
        }

        public Stream InvokeStream(Route route, params object[] args)
        {
            if (!Connected && !Authenticated) throw new NotSupportedException("Client is not connected or not authenticated.");

            var message = new StartInvokeStreamNetworkMessage(route, args);
            var stream = new DataStream(message.Id, _channel);
            
            _resultCallbacks.TryAdd(message.Id, msg =>
            {
                switch (msg)
                {
                    case DataStreamNetworkMessage data:
                        (stream as IMessageHandler).Receive(data, _channel);
                        break;
                    case EndInvokeStreamNetworkMessage end:
                        (stream as IMessageHandler).Receive(end, _channel);
                        _resultCallbacks.TryRemove(message.Id, out _);
                        break;
                }
            });
            _channel.Send(message);

            return stream;
        }

        public void Use(string name)
        {
            if (!Connected) throw new NotSupportedException("Client is disconnected.");
            if (IsUse) return;

            var req = new RequestEnvironmentUseNetworkMessage(name);
            AutoResetEvent restEvent = new AutoResetEvent(false);
            _resultCallbacks.TryAdd(req.Id, msg =>
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
                 _resultCallbacks.TryRemove(req.Id, out _);
                 restEvent.Set();
             });
            _channel.Send(req);
            restEvent.WaitOne();
            
        }

        public TResponce Invoke<TRequest, TResponce>(Route route, TRequest request)
        {
            if (!Connected && !Authenticated) throw new NotSupportedException("Client is not connected or not authenticated.");
            if (!IsUse) throw new NotSupportedException("First need to choose a database. Need call method Use.");

            TResponce responce = default(TResponce);
            Exception exception = null;
            var message = new RequestInvokeUnaryNetworkMessage(route, request);

            AutoResetEvent restEvent = new AutoResetEvent(false);

            _resultCallbacks.TryAdd(message.Id, (msg) =>
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

                _resultCallbacks.TryRemove(message.Id, out _);
                restEvent.Set();
            });
            _channel.Send(message);

            restEvent.WaitOne();
            if (exception != null) throw exception;

            return responce;

        }

        public async Task<TResponce> InvokeAsync<TRequest, TResponce>(Route route, TRequest request)
        {

            return await Task.Factory.StartNew(() => Invoke<TRequest, TResponce>(route, request)
            , TaskCreationOptions.LongRunning);
        }



      
        public void Disconnect()
        {
            Connected = false;
            _channel.Stop();
        }

        public void Dispose()
        {
            Disconnect();
            _tcpClient.Dispose();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public Stream GetStream()
        {
            return _tcpClient.GetStream();
        }

        public void Open(TcpClient client)
        {
            throw new NotImplementedException();
        }

        public void SetRemover(IDisposable remover)
        {
            throw new NotImplementedException();
        }
    }
}
