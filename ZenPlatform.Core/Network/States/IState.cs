using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Core.Tools;

namespace ZenPlatform.Core.Network.States
{
    public interface IState: IConnectionObserver<IConnectionContext>
    {
       // void OnNext(IChannel channel, INetworkMessage message, TCPServerConnection context);
    }
}
