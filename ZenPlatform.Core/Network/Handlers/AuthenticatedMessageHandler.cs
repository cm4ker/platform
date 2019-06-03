using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Sessions;

namespace ZenPlatform.Core.Network.Handlers
{
    public class AuthenticatedMessageHandler : MulticastMessageHandler
    {
        private readonly IEnvironment _environment;
        private readonly ISession _session;
        public AuthenticatedMessageHandler(IUser user, IEnvironment environment)
        {
            _environment = environment;
            _session = environment.CreateSession(user);
            
            
            Subscribe(new StreamMessageHandler(_session, environment));
            Subscribe(new InvokeMessageHandler(_session, environment));
        }
    }
}
