using System.Net;

namespace ZenPlatform.Core.Network
{

    public interface IUserListener : IListener { };
    public interface IListener
    {
        void Start(IPEndPoint endPoint);
        void Stop();
    }
}