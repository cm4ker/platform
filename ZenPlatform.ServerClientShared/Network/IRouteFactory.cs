using ZenPlatform.Core.Contracts;

namespace ZenPlatform.Core.Network
{
    public interface IRouteFactory
    {
        Route Create(string path);
    }
}