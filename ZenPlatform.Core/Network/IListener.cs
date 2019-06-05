using System.Net;

namespace ZenPlatform.Core.Network
{
    public interface IListener
    {
        void Start(IPEndPoint endPoint);
        void Stop();
    }
}