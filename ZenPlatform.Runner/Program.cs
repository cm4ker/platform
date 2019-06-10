using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using ZenPlatform.Core;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Network;
using ZenPlatform.ServerClientShared.Network;
using ZenPlatform.ServerClientShared;
using ZenPlatform.ServerClientShared.DI;
using ZenPlatform.Core.Logging;
using System.Configuration;
using ZenPlatform.ServerClientShared.Logging;
using ZenPlatform.Core.Network.Handlers;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Data;

namespace ZenPlatform.Runner
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    //config.AddEnvironmentVariables();
                    config.AddXmlFile("App.config", false, true);
                })
                /*
                .ConfigureLogging((hostContext, loggingBuilder) =>
                {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                    loggingBuilder.AddNLog(hostContext.Configuration);
                })
                */
                .ConfigureServices((hostContext, services) =>
                {
                    AppConfig config = new AppConfig();
                    hostContext.Configuration.GetSection("Runner").Bind(config);

                    services.AddConfig(config.AccessPoint);
                    services.AddConfig(config.Environments);

                    //services.AddSingleton<IInvokeServiceManager, InvokeServiceManager>();

                    services.AddTransient(typeof(ILogger<>), typeof(NLogger<>));
                    services.AddScoped<IInvokeService, InvokeService>();
                    services.AddTransient(typeof(IConnection<>), typeof(UserConnection<>));
                    services.AddTransient<IChannel, Channel>();
                    services.AddSingleton<IAccessPoint, UserAccessPoint>();
                    services.AddSingleton<ITaskManager, TaskManager>();
                    services.AddTransient<IMessagePackager, SimpleMessagePackager>();
                    services.AddTransient<ISerializer, HyperionSerializer>();
                    services.AddSingleton<IDependencyResolver, MicrosoftDependencyResolver>();
                    services.AddSingleton<IEnvironmentManager, EnvironmentManager>();
                    services.AddScoped<IEnvironment, WorkEnvironment>();
                    //services.AddTransient<IUserMessageHandler, UserMessageHandler>();
                    services.AddTransient<IUserMessageHandler, UserConnectedMessageHandler>();
                    services.AddScoped<IAuthenticationManager, AuthenticationManager>();
                    services.AddScoped<IDataContextManager, DataContextManager>();
                    services.AddScoped<IUserManager, UserManager>();

                    services.AddSingleton<IHostedService, RunnerService>();
                });

            await builder.RunConsoleAsync();
        }
    }
}