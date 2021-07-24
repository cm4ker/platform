using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using Aquila.Core.Tools;
using Aquila.Logging;

namespace Aquila.Core.Network
{
    /// <summary>
    /// Connection via TCP proto
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
            _logger.Info("Client '{0}' disconnected. {1}", _client.RemoteEndPoint, "Cause: COMPLETED");
            //throw new Exception();
            Close();
        }

        public virtual void OnError(Exception error)
        {
            _logger.Info("Client '{0}' disconnected: '{1}'", _client.RemoteEndPoint, error.Message);
            throw error;
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


    // public class SSHTransportClient : ITransportClient
    // {
    //     private readonly SshClient _client;
    //     private SSHClientShellStream _stream;
    //
    //
    //     public SSHTransportClient(SshClient client)
    //     {
    //         _client = client;
    //     }
    //
    //     public Stream GetStream()
    //     {
    //         if (_stream == null)
    //             _stream = new SSHClientShellStream(_client.CreateShellStream("client", 300,
    //                 300, 300,
    //                 300, 213));
    //         return _stream;
    //     }
    //
    //     public string RemoteEndPoint => $"{_client.ConnectionInfo.Host}:{_client.ConnectionInfo.Port}";
    //
    //     public void Close()
    //     {
    //         _client.Disconnect();
    //     }
    //
    //     public bool IsConnected => _client.IsConnected;
    //
    //     public void Dispose()
    //     {
    //         _stream?.Dispose();
    //         _client?.Dispose();
    //     }
    // }

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

    //
    // public class SSHTransportServer : ITransportClient
    // {
    //     private SessionChannel _sessionChannel;
    //
    //     public SSHTransportServer(SessionChannel sessionChannel)
    //     {
    //         _sessionChannel = sessionChannel;
    //     }
    //
    //     public void Dispose()
    //     {
    //     }
    //
    //     public Stream GetStream()
    //     {
    //         return new SSHStreamFacade(_sessionChannel);
    //     }
    //
    //     public bool IsConnected => true;
    //     public string RemoteEndPoint => "Server transport";
    //
    //     public void Close()
    //     {
    //         _sessionChannel.SendClose();
    //     }
    // }
    //
    // public class SSHStreamFacade : Stream
    // {
    //     private readonly SessionChannel _sessionChannel;
    //     private readonly MemoryStream _bufferStream;
    //     private AutoResetEvent _waitReceive;
    //     private readonly object _readLock = new object();
    //
    //     public SSHStreamFacade(SessionChannel sessionChannel)
    //     {
    //         _sessionChannel = sessionChannel;
    //         _sessionChannel.DataReceived += SessionChannelOnDataReceived;
    //         _bufferStream = new MemoryStream();
    //         _waitReceive = new AutoResetEvent(false);
    //     }
    //
    //     public override void Flush()
    //     {
    //         _sessionChannel.SendClose();
    //     }
    //
    //     public override int Read(byte[] buffer, int offset, int count)
    //     {
    //         _waitReceive.WaitOne();
    //
    //         int readbyte = 0;
    //         lock (_readLock)
    //         {
    //             readbyte = _bufferStream.Read(buffer, offset, count);
    //         }
    //
    //         return readbyte;
    //     }
    //
    //     private void SessionChannelOnDataReceived(object sender, byte[] e)
    //     {
    //         lock (_readLock)
    //         {
    //             var pos = _bufferStream.Position;
    //             _bufferStream.Seek(0, SeekOrigin.End);
    //             _bufferStream.Write(e);
    //             _bufferStream.Seek(pos, SeekOrigin.Begin);
    //             _waitReceive.Set();
    //         }
    //     }
    //
    //     public override long Seek(long offset, SeekOrigin origin)
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public override void SetLength(long value)
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public override void Write(byte[] buffer, int offset, int count)
    //     {
    //         _sessionChannel.SendData(buffer);
    //     }
    //
    //     public override bool CanRead => true;
    //     public override bool CanSeek => false;
    //     public override bool CanWrite => true;
    //     public override long Length => throw new NotImplementedException();
    //
    //     public override long Position
    //     {
    //         get => throw new NotImplementedException();
    //         set => throw new NotImplementedException();
    //     }
    // }
    //
    // public class SSHClientShellStream : Stream
    // {
    //     private readonly ShellStream _ss;
    //     private AutoResetEvent _read;
    //
    //     public SSHClientShellStream(ShellStream ss)
    //     {
    //         _ss = ss;
    //         _read = new AutoResetEvent(false);
    //         _ss.DataReceived += SsOnDataReceived;
    //     }
    //
    //     private void SsOnDataReceived(object sender, ShellDataEventArgs e)
    //     {
    //         _read.Set();
    //     }
    //
    //     public override void Flush()
    //     {
    //         _ss.Flush();
    //     }
    //
    //     public override int Read(byte[] buffer, int offset, int count)
    //     {
    //         _read.WaitOne();
    //         return _ss.Read(buffer, offset, count);
    //     }
    //
    //     public override long Seek(long offset, SeekOrigin origin)
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public override void SetLength(long value)
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public override void Write(byte[] buffer, int offset, int count)
    //     {
    //         _ss.Write(buffer, offset, count);
    //         _ss.Flush();
    //     }
    //
    //     public override bool CanRead => _ss.CanRead;
    //     public override bool CanSeek => _ss.CanSeek;
    //     public override bool CanWrite => _ss.CanWrite;
    //     public override long Length => _ss.Length;
    //
    //     public override long Position
    //     {
    //         get => _ss.Position;
    //         set => _ss.Position = value;
    //     }
    // }
}