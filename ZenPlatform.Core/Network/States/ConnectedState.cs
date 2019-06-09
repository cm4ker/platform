using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.ServerClientShared.Network;

namespace ZenPlatform.Core.Network.States
{
    public class ConnectedState : IState
    {
        public void OnNext(INetworkMessage message, ConnectionContext context)
        {

            switch (message)
            {
                case RequestEnvironmentUseNetworkMessage msg:
                    msg.UseEnvironment(context.EnvironmentManager, (env) =>
                    {
                        context.Environment = env;

                        context.State = new EnvironmentState();
                    }).PipeTo(msg.Id, context.Channel);
                    break;

                case RequestEnvironmentListNetworkMessage msg:
                    msg.GetEnvironmentList(context.EnvironmentManager).PipeTo(msg.Id, context.Channel);
                    break;
            }
        }
    }
}
