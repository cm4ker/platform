using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.ServerClientShared.Network;

namespace ZenPlatform.Core.Network.States
{
    public class NullState : IState
    {
        public void OnNext(INetworkMessage message, ConnectionContext context)
        {
            
        }
    }
}
