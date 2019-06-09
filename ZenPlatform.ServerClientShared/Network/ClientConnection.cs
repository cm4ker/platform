using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ZenPlatform.ServerClientShared.Network
{
    public class ClientConnection : IConnection
    {
        private TcpClient _tcpClient;
        private IDisposable _remover;

        public ConnectionInfo Info => throw new NotImplementedException();

        public bool Opened => _tcpClient == null ? false : _tcpClient.Connected;

        public void Close()
        {
            _remover?.Dispose();
            _tcpClient.Close();
            _tcpClient.Dispose();
        }

        public void Dispose()
        {
            Close();
        }

        public Stream GetStream()
        {
            return _tcpClient.GetStream();
        }

        public void Open(TcpClient client)
        {
            _tcpClient = client;
        }

        public void SetRemover(IDisposable remover)
        {
            _remover = remover;
        }
    }
}
