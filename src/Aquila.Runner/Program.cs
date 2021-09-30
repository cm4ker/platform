using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Aquila.Core;
using Aquila.Core.Instance;
using Aquila.Core.Network;
using Aquila.Core.Serialisers;
using Aquila.Core.Authentication;
using Aquila.Data;
using Aquila.Core.CacheService;
using Aquila.Core.Settings;
using Aquila.Core.ClientServices;
using Aquila.Core.Assemlies;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Authentication;
using Aquila.Core.Contracts.Instance;
using Aquila.Core.Contracts.Network;
using Aquila.Logging;
using Aquila.Migrations;
using Aquila.Networking;
using Aquila.Shell;
using Aquila.WebServiceCore;
using Microsoft.AspNetCore.Hosting;

namespace Aquila.Runner
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).RunConsoleAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostContext, config) => { config.AddXmlFile("App.config", false, true); })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTransient(typeof(ILogger<>), typeof(SimpleConsoleLogger<>));
                    services.AddSingleton<IHostedService, AquilaHostedService>();
                    services.AddSingleton<IWebHost, AquilaWebHost>();
                });


            return builder;
        }
    }
}