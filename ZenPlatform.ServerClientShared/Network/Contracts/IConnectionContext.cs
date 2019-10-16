using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ZenPlatform.Core.Network
{
    public interface IConnectionContext
    {
        IConnection Connection { get; }
    }


    public interface ITransportClientFactory
    {
        ITransportClient Create(IPEndPoint endPoint);
    }
}