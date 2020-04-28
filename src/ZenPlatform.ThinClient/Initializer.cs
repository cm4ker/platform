using System.IO;
using System.Net;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ZenPlatform.Core.Assemlies;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.ClientServices;
using ZenPlatform.Core.Contracts;
using ZenPlatform.Core.Logging;
using ZenPlatform.Core.Network;
using ZenPlatform.Core.Network.Contracts;
using ZenPlatform.Core.Settings;

namespace ZenPlatform.ThinClient
{
    public class FakePlatformClient : IPlatformClient
    {
        public bool IsConnected => true;

        public IConnectionInfo Info { get; }

        public void Close()
        {
        }

        public void Connect(IPEndPoint endPoint)
        {
            throw new System.NotImplementedException();
        }

        public TResponse Invoke<TResponse>(Route route, params object[] args)
        {
            throw new System.NotImplementedException();
        }

        public Stream InvokeAsStream(Route route, params object[] args)
        {
            throw new System.NotImplementedException();
        }

        public T GetService<T>()
        {
            throw new System.NotImplementedException();
        }

        public bool IsAuthenticated { get; }

        public bool Authenticate(IAuthenticationToken token)
        {
            throw new System.NotImplementedException();
        }

        public string Database { get; }
        public bool IsUse { get; }

        public bool Use(string name)
        {
            throw new System.NotImplementedException();
        }
    }

    public class FakeClientPlatformContext : IClientPlatformContext
    {
        public FakeClientPlatformContext(IPlatformClient client)
        {
            Client = client;
        }

        public Assembly LoadMainAssembly()
        {
            return typeof(EntryPoint).Assembly;
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


            services.AddSingleton<IClientPlatformContext, ClientPlatformContext>();
            services.AddSingleton<IPlatformClient, Client>();
            //services.AddSingleton<IProtocolClient, Client>();
            services.AddTransient(typeof(ILogger<>), typeof(SimpleConsoleLogger<>));
            services.AddSingleton<PlatformAssemblyLoadContext>();
            services.AddSingleton<IClientAssemblyManager, PlatformClientAssemblyManager>();
            services.AddSingleton<ITransportClientFactory, TCPTransportClientFactory>();
            //services.AddSingleton<IPlatformClient, Client>();

            services.AddSingleton(factory =>
            {
                var client = factory.GetRequiredService<IPlatformClient>();
                return client.GetService<IAssemblyManagerClientService>();
            });

            return services.BuildServiceProvider();
        }
    }
}