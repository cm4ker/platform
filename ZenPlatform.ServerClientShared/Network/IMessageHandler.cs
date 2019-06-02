using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.ServerClientShared.Network
{
    
    public interface IMessageHandler
    {
        void  Receive(object message, IChannel channel);
    }
}
