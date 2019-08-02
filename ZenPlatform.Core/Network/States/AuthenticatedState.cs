using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Core.Network.States
{
    public class AuthenticatedState
    {
        public void OnNext(IChannel channel, INetworkMessage message, TCPServerConnection context)
        {
            switch (message)
            {
                case RequestInvokeUnaryNetworkMessage msg:
                  //  msg.Invoke(context.Environment.InvokeService, context.Session).PipeTo(msg.Id, context.Channel);
                    
                    break;
                case StartInvokeStreamNetworkMessage msg:
                  //  msg.InvokeStream(context.Environment.InvokeService, context.Session, context.Channel).PipeTo(msg.Id, context.Channel);
                    break;
            }
        }
    }
}
