using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using ZenPlatform.Core.DI;
using ZenPlatform.Core.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Net;
using Mono.Cecil;
using Renci.SshNet;
using ZenPlatform.Core.Tools;
using ZenPlatform.Core.Authentication;

namespace ZenPlatform.Core.Network
{
    /// <summary>
    /// Олицетворяет соединение по протоколу TCP
    /// </summary>
    public abstract class Connection : IDisposable, IConnection, IObserver<INetworkMessage>
    {
        private ITransportClient _client;
        private IDisposable _unsubscriber;
        protected List<IConnectionObserver<IConnectionContext>> _connectionObservers;

        private readonly ILogger _logger;

        public bool Opened { get; private set; }

        public IChannel Channel { get; }

        public ConnectionInfo Info => throw new NotImplementedException();

        public Connection(ILogger logger, ITransportClient client, IChannelFactory channelFactory)
        {
            Channel = channelFactory.CreateChannel();
            _unsubscriber = Channel.Subscribe(this);
            _logger = logger;
            _client = client;
            _client = client ?? throw new ArgumentNullException(nameof(client));
            if (!client.IsConnected) throw new InvalidOperationException("Client must be connected.");

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
                if (_client?.IsConnected == true)
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
            _logger.Info("Client '{0}' disconnected.", _client.RemoteEndPoint);
            Close();
        }

        public virtual void OnError(Exception error)
        {
            _logger.Info("Client '{0}' disconnected: '{1}'", _client.RemoteEndPoint, error.Message);
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

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }


    public interface ITransportClient : IDisposable
    {
        Stream GetStream();

        bool IsConnected { get; }

        string RemoteEndPoint { get; }

        void Close();
    }


    public class SSHTransportClient : ITransportClient
    {
        private readonly SshClient _client;
        private ShellStream _stream;


        public SSHTransportClient(SshClient client)
        {
            _client = client;
        }

        public Stream GetStream()
        {
            if (_stream == null)
                _stream = _client.CreateShellStream("someTerminal", UInt32.MaxValue, UInt32.MaxValue, UInt32.MaxValue,
                    UInt32.MaxValue, Int32.MaxValue);
            return _stream;
        }

        public string RemoteEndPoint => $"{_client.ConnectionInfo.Host}:{_client.ConnectionInfo.Port}";

        public void Close()
        {
            _client.Disconnect();
        }

        public bool IsConnected => _client.IsConnected;

        public void Dispose()
        {
            _stream?.Dispose();
            _client?.Dispose();
        }
    }

    public class TCPTransportClient : ITransportClient
    {
        private readonly TcpClient _client;
        private NetworkStream _stream;


        public TCPTransportClient(TcpClient client)
        {
            _client = client;
        }

        public Stream GetStream()
        {
            if (_stream == null)
                _stream = _client.GetStream();
            return _stream;
        }

        public string RemoteEndPoint => $"{_client.Client.RemoteEndPoint}";

        public void Close()
        {
            _client.Close();
        }

        public bool IsConnected => _client.Connected;

        public void Dispose()
        {
            _stream?.Dispose();
            _client?.Dispose();
        }
    }
}