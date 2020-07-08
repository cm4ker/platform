using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Aquila.Core;
using Aquila.Core.Environment;
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
using Aquila.Core.Contracts.Environment;
using Aquila.Core.Contracts.Network;
using Aquila.Logging;
using Aquila.Migrations;
using Aquila.Networking;
using Aquila.WebServiceCore;
using Microsoft.AspNetCore.Hosting;

namespace Aquila.Runner
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
                    services.AddTransient(typeof(ILogger<>), typeof(SimpleConsoleLogger<>));
                    services.AddTransient<IDatabaseNetworkListener, TCPListener>();

                    services.AddScoped<IInvokeService, InvokeService>();
                    services.AddTransient<INetworkListener, TCPListener>();
                    services.AddTransient<IChannel, Channel>();
                    services.AddSingleton<IAccessPoint, UserAccessPoint>();
                    services.AddSingleton<ITaskManager, TaskManager>();
                    services.AddTransient<IMessagePackager, SimpleMessagePackager>();
                    services.AddTransient<ISerializer, ApexSerializer>();
                    services.AddTransient<UserConnectionFactory>();
                    services.AddTransient<ServerConnectionFactory>();
                    services.AddTransient<IChannelFactory, ChannelFactory>();
                    // services.AddScoped<IAdminToolsClientService, AdminToolsClientService>();
                    // services.AddScoped<IAssemblyManagerClientService, AssemblyManagerClientService>();
                    // services.AddScoped<IAssemblyManager, AssemblyManager>();
                    // services.AddScoped<IConfigurationManager, ConfigurationManager>();
                    //services.AddScoped<IXCCompiller, XCCompiler>();
                    services.AddScoped<ILinkFactory, LinkFactory>();
                    //services.AddScoped<IAssemblyPlatform, DnlibAssemblyPlatform>();
                    // services.AddScoped<IAssemblyStorage, DatabaseAssemblyStorage>();
                    //services.AddSingleton<IConfigurationManipulator, XCConfManipulator>();
                    services.AddSingleton<IStartupService, StartupServiceImpl>();
                    services.AddScoped<MigrationManager>();


                    // services.AddSingleton<ITestProxyService, TestProxyService>();
                    services.AddSingleton<IPlatformEnvironmentManager, EnvironmentManager>();

                    //services.AddScoped<ITestEnvironment, TestEnvironment>();
                    services.AddScoped<IAdminEnvironment, AdminEnvironment>();
                    services.AddScoped<IWorkEnvironment, DatabaseTestEnvironment>();

                    services.AddSingleton<ICacheService, DictionaryCacheService>();


                    //services.AddTransient<IUserMessageHandler, UserMessageHandler>();
                    services.AddScoped<IAuthenticationManager, AuthenticationManager>();
                    services.AddScoped<DataContextManager>();
                    services.AddScoped<IUserManager, UserManager>();

                    services.AddSingleton<IHostedService, RunnerService>();
                    services.AddSingleton<IWebHost, RunnerWebService>();
                });

            await builder.RunConsoleAsync();
        }
    }
}