using Aquila.Core.Contracts;

namespace Aquila.Core.Network
{
    public interface IRouteFactory
    {
        Route Create(string path);
    }
}