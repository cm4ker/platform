using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ZenPlatform.Core.Assemlies;
using ZenPlatform.Core.ClientServices;
using ZenPlatform.Core.Contracts.Network;
using ZenPlatform.Core.Logging;
using ZenPlatform.Core.Network;
using ZenPlatform.Core.Network.Contracts;
using ZenPlatform.Core.Settings;

namespace ZenPlatform.ThinClient
{
    public class FakeClientPlatformContext : IClientPlatformContext
    {
        public Assembly LoadMainAssembly()
        {
            return null;// typeof(FakeClass).Assembly;
        }

        public Assembly MainAssembly { get; }
        public IPlatformClient Client { get; }

        public void Connect(DatabaseConnectionSettings connectionSettings)
        {
        }

        public void Login(string name, string password)
        {
        }
    }

    public static class Initializer
    {
        public static ServiceProvider GetServerService()
        {
            IServiceCollection services = new ServiceCollection();

            return services.BuildServiceProvider();
        }

        public static ServiceProvider GetClientService()
        {
            IServiceCollection services = new ServiceCollection();


            services.AddSingleton<IClientPlatformContext, FakeClientPlatformContext>();
            services.AddSingleton<IProtocolClient, Client>();
            services.AddTransient(typeof(ILogger<>), typeof(SimpleConsoleLogger<>));
            services.AddSingleton<PlatformAssemblyLoadContext>();
            services.AddSingleton<IClientAssemblyManager, PlatformClientAssemblyManager>();
            services.AddSingleton<ITransportClientFactory, SSHTransportClientFactory>();
            services.AddSingleton<IPlatformClient, Client>();

            services.AddSingleton(factory =>
            {
                var client = factory.GetRequiredService<IProtocolClient>();
                return client.GetService<IAssemblyManagerClientService>();
            });

            return services.BuildServiceProvider();
        }
    }
}