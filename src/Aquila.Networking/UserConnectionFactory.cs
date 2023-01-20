using System;
using Aquila.Core.Instance;
using Microsoft.Extensions.DependencyInjection;

namespace Aquila.Core.Network
{
    /// <summary>
    /// Service connection factory
    /// </summary>
    public class UserConnectionFactory : ServerConnectionFactory
    {
        public UserConnectionFactory(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Connection CreateConnection(ITransportClient tcpClient)
        {
            return CreateConnection(tcpClient, new AqFilteredInstanceManager(
                _serviceProvider.GetRequiredService<AqInstanceManager>(),
                env => env is AqInstance));
        }
    }
}