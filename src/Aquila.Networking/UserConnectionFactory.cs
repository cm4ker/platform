using System;
using Microsoft.Extensions.DependencyInjection;
using Aquila.Core.Contracts.Environment;
using Aquila.Core.Environment;

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
            return CreateConnection(tcpClient, new FilteredEnvironmentManager(
                _serviceProvider.GetRequiredService<IPlatformEnvironmentManager>(),
                env => env.GetType() == typeof(WorkEnvironment)));
        }
    }
}