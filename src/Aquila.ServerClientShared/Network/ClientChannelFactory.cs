using System;
using System.Collections.Generic;
using System.Text;
using Aquila.Core.Logging;
using Aquila.Core.Serialisers;

namespace Aquila.Core.Network
{
    public class ClientChannelFactory : IChannelFactory
    {
        public IChannel CreateChannel()
        {
            return new Channel(new SimpleMessagePackager(new ApexSerializer()), new SimpleConsoleLogger<Channel>());
        }
    }
}
