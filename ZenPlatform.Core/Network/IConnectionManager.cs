

namespace ZenPlatform.Core.Network
{
    public interface IConnectionManager
    {
        void AddConnection(TCPServerConnection connection);
    }
}