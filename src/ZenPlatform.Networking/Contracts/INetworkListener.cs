using System;
using System.Net;

namespace ZenPlatform.Core.Network
{

    public interface ITerminalNetworkListener : INetworkListener { };
    public interface IDatabaseNetworkListener : INetworkListener { };
    public interface INetworkListener : IDisposable
    {
        void Start(IPEndPoint endPoint);
        void Stop();
    }
}