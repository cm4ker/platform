using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Sessions;
using ZenPlatform.ServerClientShared.Network;

namespace ZenPlatform.Core.Network.Handlers
{
    public class StreamMessageHandler : GuidMessageHandler
    {
        private readonly IEnvironment _environment;
        private readonly ISession _session;

        public StreamMessageHandler(ISession session, IEnvironment environment)
        {
            _environment = environment;
            _session = session;

        }

        public override void ReceiveMessage(object message, IChannel channel, bool processed)
        {

            switch (message)
            {
                case StartInvokeStreamNetworkMessage start:
                    var stream = new DataStream(start.Id, channel);
                    AddHandlerByGuid(start.Id, stream);
                    _environment.InvokeService.InvokeStream(start.Route, _session, stream, start.Request);
                    break;
                case EndInvokeStreamNetworkMessage end:
                    DelHandlerByGuid(end.Id);
                    break;
            }
        }
    }
}
