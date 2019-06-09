using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.ServerClientShared.Network;

namespace ZenPlatform.Core.Network.States
{
    public interface IState
    {
        void OnNext(INetworkMessage message, ConnectionContext context);
    }
}
