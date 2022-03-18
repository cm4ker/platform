using Aquila.Core.Serialisers;
using Aquila.Logging;

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