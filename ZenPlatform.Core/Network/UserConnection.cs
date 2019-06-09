using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Network.States;
using ZenPlatform.ServerClientShared.Logging;
using ZenPlatform.ServerClientShared.Network;

namespace ZenPlatform.Core.Network
{
    public class UserConnection : Connection, IUserConnection
    {
        private readonly ConnectionContext _connectionContext;
        public UserConnection(ILogger<UserConnection> logger, IChannel channel, IEnvironmentManager environmentManager) 
            : base(logger, channel)
        {
            _connectionContext = new ConnectionContext(environmentManager, channel);

            _connectionContext.State = new ConnectedState();
        }

        public override void OnNext(INetworkMessage value)
        {
            _connectionContext.OnNext(value);

        }
    }
}
