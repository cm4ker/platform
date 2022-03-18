using Aquila.Core.Contracts;

namespace Aquila.Core.Network
{
    public class ServerConnectionContext : IConnectionContext
    {
        public ISession Session { get; set; }
        public IConnection Connection { get; set; }
    }
}