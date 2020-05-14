using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Dnlib;
using Aquila.Core.Environment;
using Aquila.Core.Network;
using Aquila.Core.Serialisers;
using Aquila.Core.Logging;
using Aquila.Core.Authentication;
using Aquila.Data;
using Aquila.Core.CacheService;
using Aquila.Core.Settings;
using Aquila.Core.Tools;
using Aquila.Core.ClientServices;
using Aquila.Core.Assemblies;
using Aquila.Core.Configuration;
using Aquila.Compiler.Platform;
using Aquila.Configuration;
using Aquila.Configuration.Contracts;
using Aquila.Configuration.Contracts.Data;
using Aquila.Core.Assemlies;
using Aquila.Core.Contracts.Environment;
using Aquila.Core.DI;
using Aquila.Migration;
using Aquila.Networking;

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