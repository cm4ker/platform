using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Core.Environment;
using ZenPlatform.ServerClientShared.Network;

namespace ZenPlatform.Core.Network.Handlers
{
    public class EnvironmentUsedMessageHandler : IMessageHandler
    {
        private readonly IEnvironment _environment;
        public EnvironmentUsedMessageHandler(IEnvironment environment)
        {
            _environment = environment;
        }

        public void Receive(object message, IChannel channel)
        {
            switch(message)
            {
                case RequestAuthenticationNetworkMessage auth:
                    try
                    {
                        var user = _environment.AuthenticationManager.Authenticate(auth.Token);
                        channel.SetHandler(new AuthenticatedMessageHandler(user, _environment));
                        channel.Send(new ResponceAuthenticationNetworkMessage(auth));
                    } catch (Exception ex)
                    {
                        channel.Send(new ErrorNetworkMessage(auth.Id, "Authentication error", ex));
                    }
                     
                    break;
            }
        }
    }
}
