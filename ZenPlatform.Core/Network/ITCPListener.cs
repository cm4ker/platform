﻿using System.Net;

namespace ZenPlatform.Core.Network
{

    public interface ITCPListener
    {
        void Start(IPEndPoint endPoint, ServerConnectionFactory connectionFactory);
        void Stop();
    }
}