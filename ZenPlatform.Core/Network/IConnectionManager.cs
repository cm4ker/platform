using ZenPlatform.ServerClientShared.Network;

namespace ZenPlatform.Core.Network
{
    public interface IConnectionManager
    {
        void AddConnection(IConnection connection);
    }
}