using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Sessions;
using ZenPlatform.ServerClientShared.Network;

namespace ZenPlatform.Core.Network.States
{
    public class ConnectionContext
    {
        public ConnectionContext(IEnvironmentManager environmentManager, IChannel channel)
        {
            State = new NullState();
            EnvironmentManager = environmentManager;
            Channel = channel;
        }
        public IState State { get; set; }
        public IEnvironmentManager EnvironmentManager { get; set; }
        public IChannel Channel { get; set; }
        public IEnvironment Environment { get; set; }
        public ISession Session { get; set; }

        public void OnNext(INetworkMessage message)
        {
            State.OnNext(message, this);
        }
    }
}
