using System;
using System.Net;

namespace ZenPlatform.Core.Network
{
    public interface INetworkListener : IDisposable
    {
        void Start(IPEndPoint endPoint, ServerConnectionFactory connectionFactory);
        void Stop();
    }
}