using Aquila.Core.Contracts;

namespace Aquila.Core.Network
{
    public class ServerConnectionContext : IConnectionContext
    {
        public IConnection Connection { get; set; }
    }
}