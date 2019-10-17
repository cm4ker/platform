using Microsoft.Extensions.DependencyInjection;
using ZenPlatform.Core.Assemlies;
using ZenPlatform.Core.ClientServices;
using ZenPlatform.Core.Logging;
using ZenPlatform.Core.Network;
using ZenPlatform.Core.Network.Contracts;

namespace ZenPlatform.ThinClient
{
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


            services.AddSingleton<PlatformClient>();
            services.AddSingleton<IProtocolClient, Client>();
            services.AddTransient(typeof(ILogger<>), typeof(SimpleConsoleLogger<>));
            services.AddSingleton<PlatformAssemblyLoadContext>();
            services.AddSingleton<IClientAssemblyManager, ClientAssemblyManager>();


            services.AddSingleton(factory =>
            {
                var client = factory.GetRequiredService<IProtocolClient>();
                return client.GetService<IAssemblyManagerClientService>();
            });

            return services.BuildServiceProvider();
        }
    }
}