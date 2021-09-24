using System;
using Microsoft.Extensions.DependencyInjection;
using Aquila.Core.Contracts.Instance;
using Aquila.Core.Instance;

namespace Aquila.Core.Network
{
    /// <summary>
    /// Фабрика создания серверных подключений для полльзователей исключительно к рабочим средам
    /// </summary>
    public class UserConnectionFactory : ServerConnectionFactory
    {
        public UserConnectionFactory(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Connection CreateConnection(ITransportClient tcpClient)
        {
            return CreateConnection(tcpClient, new FilteredInstanceManager(
                _serviceProvider.GetRequiredService<IPlatformInstanceManager>(),
                env => env is IPlatformInstance));
        }
    }
}