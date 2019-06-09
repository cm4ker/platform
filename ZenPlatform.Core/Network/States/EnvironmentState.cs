using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.ServerClientShared.Network;

namespace ZenPlatform.Core.Network.States
{
    public class EnvironmentState : IState
    {
        public void OnNext(INetworkMessage message, ConnectionContext context)
        {
            if (message is RequestAuthenticationNetworkMessage msg)
            {
                msg.Authentication(context.Environment.AuthenticationManager, (u) =>
                {
                    context.Session = context.Environment.CreateSession(u);
                    context.State = new AuthenticatedState();

                }).PipeTo(msg.Id, context.Channel);
            }
        }
    }
}
