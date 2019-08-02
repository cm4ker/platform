using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Core.Network.States
{
    public class NullState 
    {
        public void OnNext(IChannel channel, INetworkMessage message, TCPServerConnection context)
        {
            
        }
    }
}
