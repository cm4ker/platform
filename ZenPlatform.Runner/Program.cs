using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using ZenPlatform.Core;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Network;
using ZenPlatform.Core.Serialisers;
using ZenPlatform.Core.Logging;
using System.Configuration;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Data;
using DryIoc;
using ZenPlatform.Core.CacheService;
using ZenPlatform.Core.Settings;
using ZenPlatform.Core.Tools;
using ZenPlatform.Core.ClientServices;

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

                //.UseServiceProviderFactory<IContainer>(new DryIocServiceProviderFactory())
                .ConfigureServices((hostContext, services) =>
                {
                    //AppConfig config = new AppConfig();
                    //hostContext.Configuration.GetSection("Runner").Bind(config);

                    //services.AddConfig(config.AccessPoint);
                    //services.AddConfig(config.Environments);

                    services.AddSingleton<ISettingsStorage, FileSettingsStorage>();

                    
                    services.AddTransient<IConnectionManager, ConnectionManager>();
                    services.AddTransient(typeof(ILogger<>), typeof(NLogger<>));
                    services.AddScoped<IInvokeService, InvokeService>();
                    services.AddTransient<ITCPListener, TCPListener>();
                    services.AddTransient<IChannel, Channel>();
                    services.AddSingleton<IAccessPoint, UserAccessPoint>();
                    services.AddSingleton<ITaskManager, TaskManager>();
                    services.AddTransient<IMessagePackager, SimpleMessagePackager>();
                    services.AddTransient<ISerializer, ApexSerializer>();
                    services.AddTransient<UserTCPConnectionFactory>();
                    services.AddTransient<TCPConnectionFactory>();
                    services.AddTransient<IChannelFactory, ChannelFactory>();
                    services.AddScoped<IAdminToolsClientService, AdminToolsClientService>();
                    services.AddScoped<IAssemblyManagerClientService, AssemblyManagerClientService>();
                    services.AddSingleton<ITestProxyService, TestProxyService>();
                    services.AddSingleton<IEnvironmentManager, EnvironmentManager>();

                    services.AddScoped<ITestEnvironment, TestEnvironment>();
                    services.AddScoped<IAdminEnvironment, AdminEnvironment>();
                    services.AddScoped<IWorkEnvironment, WorkEnvironment>();

                    services.AddSingleton<ICacheService, DictionaryCacheService>();


                    //services.AddTransient<IUserMessageHandler, UserMessageHandler>();
                    services.AddScoped<IAuthenticationManager, AuthenticationManager>();
                    services.AddScoped<IDataContextManager, DataContextManager>();
                    services.AddScoped<IUserManager, UserManager>();

                    services.AddSingleton<IHostedService, RunnerService>();
                });

            await builder.RunConsoleAsync();
        }
    }
}