using System.Net.Sockets;

namespace ZenPlatform.Core.Network
{
    public interface ITCPConnectionFactory
    {
        TCPServerConnection CreateConnection(TcpClient tcpClient);
    }
}