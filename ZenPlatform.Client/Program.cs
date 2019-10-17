using System;
using Microsoft.Extensions.DependencyInjection;
using ZenPlatform.Core.Assemlies;
using ZenPlatform.Core.ClientServices;
using ZenPlatform.Core.Logging;
using ZenPlatform.Core.Network;
using ZenPlatform.Core.Settings;

namespace ZenPlatform.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the console client ZenPlatform");

            Console.WriteLine("Please enter the destination address or leave empty for the default config [{0}] ",
                "Some Addr");

            Console.WriteLine("Attempt to connect");


            var serv = Build();

            var client = serv.GetService<PlatformClient>();
            client.Connect(new DatabaseConnectionSettings {Address = "localhost:12345", Database = "Library"});
            client.LoadMainAssembly();
        }


        public static ServiceProvider Build()
        {
            IServiceCollection services = new ServiceCollection();


            services.AddSingleton<PlatformClient>();
            services.AddSingleton<IClient, Core.Network.Client>();
            services.AddTransient(typeof(ILogger<>), typeof(NLogger<>));
            services.AddSingleton<PlatformAssemblyLoadContext>();
            services.AddSingleton<IClientAssemblyManager, ClientAssemblyManager>();
            services.AddTransient<ITransportClientFactory, TCPTransportClientFactory>();


            services.AddSingleton(factory =>
            {
                var client = factory.GetRequiredService<IClient>();
                return client.GetService<IAssemblyManagerClientService>();
            });

            return services.BuildServiceProvider();
        }
    }
}