using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Sessions;
using ZenPlatform.ServerClientShared.Network;

namespace ZenPlatform.Core.Network.Handlers
{
    public class InvokeMessageHandler : IMessageHandler
    {
        private readonly IEnvironment _environment;
        private readonly ISession _session;

        public InvokeMessageHandler(ISession session, IEnvironment environment)
        {
            _session = session;
            _environment = environment;

        }

        public void Receive(object message, IChannel channel)
        {
            if (message is RequestInvokeUnaryNetworkMessage msg)
            {
                try
                {
                    channel.Send(new ResponceInvokeUnaryNetworkMessage(msg.Id, _environment.InvokeService.Invoke(msg.Route, _session, msg.Request).Result));
                }
                catch (Exception ex)
                {
                    channel.Send(new ErrorNetworkMessage(msg.Id, $"Error invoke method '{msg.Route}'", ex));
                }

            }
        }
    }
}
