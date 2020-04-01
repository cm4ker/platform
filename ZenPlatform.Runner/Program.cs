using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Dnlib;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Network;
using ZenPlatform.Core.Serialisers;
using ZenPlatform.Core.Logging;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Data;
using ZenPlatform.Core.CacheService;
using ZenPlatform.Core.Settings;
using ZenPlatform.Core.Tools;
using ZenPlatform.Core.ClientServices;
using ZenPlatform.Core.Assemblies;
using ZenPlatform.Core.Configuration;
using ZenPlatform.Compiler.Platform;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Core.Assemlies;
using ZenPlatform.Core.DI;
using ZenPlatform.Core.Environment.Contracts;
using ZenPlatform.Migration;
using ZenPlatform.Networking;

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
                    services.AddScoped<IAdminToolsClientService, AdminToolsClientService>();
                    services.AddScoped<IAssemblyManagerClientService, AssemblyManagerClientService>();
                    services.AddScoped<IAssemblyManager, AssemblyManager>();
                    services.AddScoped<IConfigurationManager, ConfigurationManager>();
                    services.AddScoped<IXCCompiller, XCCompiler>();
                    services.AddScoped<IAssemblyPlatform, DnlibAssemblyPlatform>();
                    services.AddScoped<IAssemblyStorage, DatabaseAssemblyStorage>();
                    services.AddSingleton<IConfigurationManipulator, XCConfManipulator>();
                    services.AddScoped<IMigrationManager, MigrationManager>();


                    services.AddSingleton<ITestProxyService, TestProxyService>();
                    services.AddSingleton<IPlatformEnvironmentManager, EnvironmentManager>();

                    //services.AddScoped<ITestEnvironment, TestEnvironment>();
                    services.AddScoped<IAdminEnvironment, AdminEnvironment>();
                    services.AddScoped<IWorkEnvironment, DatabaseTestEnvironment>();

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