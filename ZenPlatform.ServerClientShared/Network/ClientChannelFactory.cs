using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Core.Logging;
using ZenPlatform.Core.Serialisers;

namespace ZenPlatform.Core.Network
{
    public class ClientChannelFactory : IChannelFactory
    {
        public IChannel CreateChannel()
        {
            return new Channel(new SimpleMessagePackager(new ApexSerializer()), new SimpleConsoleLogger<Channel>());
        }
    }
}
