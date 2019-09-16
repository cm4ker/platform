using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using ZenPlatform.Core.DI;
using ZenPlatform.Core.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using ZenPlatform.Core.Tools;
using ZenPlatform.Core.Authentication;

namespace ZenPlatform.Core.Network
{


    public abstract class TCPConnection : IDisposable,  IConnection, IObserver<INetworkMessage>
    {
        private TcpClient _client;
        private IDisposable _unsubscriber;
        protected List<IConnectionObserver<IConnectionContext>> _connectionObservers;
        
        private readonly ILogger _logger;

        public bool Opened { get; private set; }

        public IChannel Channel { get; }

        public ConnectionInfo Info => throw new NotImplementedException();

        public TCPConnection(ILogger logger, TcpClient client, IChannelFactory channelFactory)
        {
            Channel = channelFactory.CreateChannel();
            _unsubscriber = Channel.Subscribe(this);
            _logger = logger;
            _client = client;
            _client = client ?? throw new ArgumentNullException(nameof(client));
            if (!client.Connected) throw new InvalidOperationException("Client must be connected.");

            _connectionObservers = new List<IConnectionObserver<IConnectionContext>>();
        }

        public void Open()
        {
            
            Channel.Start(_client.GetStream());

            Opened = true;
        }

        public Stream GetStream()
        {
            return _client.GetStream();
        }

        public virtual void Close()
        {
            if (Opened)
            {
                _unsubscriber?.Dispose();
                Channel?.Stop();
                if (_client?.Connected == true)
                {
                    _client.GetStream().Close();

                    _client.Close();
                }
                _client?.Dispose();
                Opened = false;
            }
        }

        public virtual void OnCompleted()
        {
            _logger.Info("Client '{0}' disconnected.", _client.Client.RemoteEndPoint);
            Close();
        }

        public virtual void OnError(Exception error)
        {
            _logger.Info("Client '{0}' disconnected: '{1}'", _client.Client.RemoteEndPoint, error.Message);
            Close();
        }

        public abstract void OnNext(INetworkMessage value);

        public IDisposable Subscribe(IConnectionObserver<IConnectionContext> observer)
        {
            _connectionObservers.Add(observer);

            return new ListRemover<IConnectionObserver<IConnectionContext>>(_connectionObservers, observer);
        }

        #region IDisposable Support
        private bool disposedValue = false; // Для определения избыточных вызовов

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Close();
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить ниже метод завершения.
                // TODO: задать большим полям значение NULL.

                disposedValue = true;
            }
        }

        // TODO: переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        // ~TCPConnection()
        // {
        //   // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
        //   Dispose(false);
        // }

        // Этот код добавлен для правильной реализации шаблона высвобождаемого класса.
        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(true);
            // TODO: раскомментировать следующую строку, если метод завершения переопределен выше.
            // GC.SuppressFinalize(this);
        }
#endregion

        
        
    }
}
