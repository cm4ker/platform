using System;
using Microsoft.Extensions.DependencyInjection;
using Aquila.ClientRuntime;
using Aquila.Core.Assemlies;
using Aquila.Core.ClientServices;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Network;
using Aquila.Core.Logging;
using Aquila.Core.Network;
using Aquila.Core.Settings;

namespace Aquila.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the console client Aquila");

            Console.WriteLine("Please enter the destination address or leave empty for the default config [{0}] ",
                "Some Addr");

            Console.WriteLine("Attempt to connect...");


            var serv = Build();

            var context = serv.GetService<ClientPlatformContext>();
            context.Connect(new DatabaseConnectionSettings {Address = "localhost:12345", Database = "Library"});

            if (context.Client.IsConnected) Console.WriteLine("Success connect!");
            else
            {
                Console.WriteLine("Connection has refused!");
                return;
            }

            Console.WriteLine("Start load assembly");
            context.LoadMainAssembly();
            Console.WriteLine("Done load assembly");

            GlobalScope.Client = context.Client;
        }

        public static ServiceProvider Build()
        {
            IServiceCollection services = new ServiceCollection();


            services.AddSingleton<ClientPlatformContext>();
            services.AddSingleton<IProtocolClient, Core.Network.Client>();
            services.AddTransient(typeof(ILogger<>), typeof(NLogger<>));
            services.AddSingleton<PlatformAssemblyLoadContext>();
            services.AddSingleton<IClientAssemblyManager, PlatformClientAssemblyManager>();
            services.AddTransient<ITransportClientFactory, TCPTransportClientFactory>();


            services.AddSingleton(factory =>
            {
                var client = factory.GetRequiredService<IProtocolClient>();
                return client.GetService<IAssemblyManagerClientService>();
            });

            return services.BuildServiceProvider();
        }
    }
}