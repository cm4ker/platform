﻿using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Aquila.Logging;
using Aquila.Web;
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
                .ConfigureAppConfiguration((_, config) => { config.AddXmlFile("App.config", false, true); })
                .ConfigureServices((_, services) =>
                {
                    services.AddTransient(typeof(ILogger<>), typeof(SimpleConsoleLogger<>));
                    services.AddSingleton<IHostedService, AquilaHostedService>();
                    services.AddSingleton<IWebHost, AquilaWebHost>();
                });

           
            
            return builder;
        }
    }
}