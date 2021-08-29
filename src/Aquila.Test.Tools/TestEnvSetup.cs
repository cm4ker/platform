using Aquila.Core.Assemlies;
using Aquila.Core.Authentication;
using Aquila.Core.CacheService;
using Aquila.Core.ClientServices;
using Aquila.Data;
using Aquila.Core.Environment;
using Xunit.Abstractions;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Authentication;
using Aquila.Core.Contracts.Instance;
using Aquila.Core.Contracts.Network;
using Aquila.Core.Network;
using Aquila.Core.Serialisers;
using Aquila.Core.Settings;
using Aquila.Core.Test.Environment;
using Aquila.Logging;
using Aquila.Migrations;
using Aquila.Networking;
using Aquila.Test.Tools.Assemblies;
using Microsoft.Extensions.DependencyInjection;

namespace Aquila.Core.Test
{
    public static class TestEnvSetup
    {
        public static ServiceProvider GetServerService(ITestOutputHelper testOutput)
        {
            IServiceCollection services = new ServiceCollection();

            services.AddTransient<IConnectionManager, ConnectionManager>();
            services.AddSingleton(testOutput);
            //services.AddTransient(typeof(ILogger<>), typeof(XUnitLogger<>));
            services.AddTransient(typeof(ILogger<>), typeof(SimpleConsoleLogger<>));
            services.AddScoped<IInvokeService, InvokeService>();

            //services.AddTransient<ITerminalNetworkListener, SSHListener>();
            services.AddTransient<IDatabaseNetworkListener, TCPListener>();

            services.AddTransient<IChannel, Channel>();
            services.AddSingleton<IAccessPoint, UserAccessPoint>();
            services.AddSingleton<ITaskManager, TaskManager>();
            services.AddTransient<IMessagePackager, SimpleMessagePackager>();
            services.AddTransient<ISerializer, ApexSerializer>();
            services.AddTransient<UserConnectionFactory>();
            services.AddTransient<ServerConnectionFactory>();
            services.AddTransient<IChannelFactory, ChannelFactory>();
            //services.AddScoped<IAdminToolsClientService, AdminToolsClientService>();
            //services.AddScoped<IAssemblyManagerClientService, AssemblyManagerClientService>();
            //services.AddSingleton<IConfigurationManipulator, XCConfManipulator>();
            //services.AddScoped<IAssemblyManager, TestAsmManager>();
            services.AddSingleton<ISettingsStorage, TestSettingsStorage>();
            //services.AddSingleton<IFileSystem, MemoryFileSystem>();
            services.AddSingleton<IAssemblyStorage, TestAssemblyStorage>();

            //services.AddScoped<IAssemblyPlatform, DnlibAssemblyPlatform>();
            //services.AddScoped<IConfigurationManager, ConfigurationManager>();
            //services.AddScoped<IXCCompiller, XCCompiler>();
            //services.AddScoped<ILinkFactory, LinkFactory>();


            //services.AddSingleton<ITestProxyService, TestProxyService>();
            services.AddSingleton<IPlatformInstanceManager, InstanceManager>();

            //services.AddScoped<ITestEnvironment, TestEnvironment>();
            //services.AddScoped<IAdminEnvironment, AdminEnvironment>();
            services.AddScoped<IWorkInstance, TestInstance>();

            services.AddSingleton<ICacheService, DictionaryCacheService>();

            //services.AddTransient<IUserMessageHandler, UserMessageHandler>();
            services.AddScoped<IAuthenticationManager, AuthenticationManager>();
            //services.AddScoped<IDataContextManager, DataContextManager>();
            services.AddScoped<IUserManager, UserManager>();


            return services.BuildServiceProvider();
        }

        //
        // public static ServiceProvider GetServerServiceWithDatabase(ITestOutputHelper testOutput)
        // {
        //     IServiceCollection services = new ServiceCollection();
        //
        //     services.AddTransient<IConnectionManager, ConnectionManager>();
        //     services.AddSingleton(testOutput);
        //     //services.AddTransient(typeof(ILogger<>), typeof(XUnitLogger<>));
        //     services.AddTransient(typeof(ILogger<>), typeof(NLogger<>));
        //     services.AddScoped<IInvokeService, InvokeService>();
        //
        //     services.AddTransient<ITerminalNetworkListener, SSHListener>();
        //     services.AddTransient<IDatabaseNetworkListener, TCPListener>();
        //
        //     services.AddTransient<IChannel, Channel>();
        //     services.AddSingleton<IAccessPoint, UserAccessPoint>();
        //     services.AddSingleton<ITaskManager, TaskManager>();
        //     services.AddTransient<IMessagePackager, SimpleMessagePackager>();
        //     services.AddTransient<ISerializer, ApexSerializer>();
        //     services.AddTransient<UserConnectionFactory>();
        //     services.AddTransient<ServerConnectionFactory>();
        //     services.AddSingleton<IConfigurationManipulator, XCConfManipulator>();
        //     services.AddTransient<IChannelFactory, ChannelFactory>();
        //     services.AddScoped<IAdminToolsClientService, AdminToolsClientService>();
        //     services.AddScoped<IAssemblyManagerClientService, AssemblyManagerClientService>();
        //
        //     services.AddScoped<IAssemblyManager, AssemblyManager>();
        //     services.AddSingleton<ISettingsStorage, TestSettingsStorage>();
        //     services.AddSingleton<IFileSystem, MemoryFileSystem>();
        //     //services.AddSingleton<IAssemblyStorage, TestAssemblyStorage>();
        //     services.AddScoped<IAssemblyStorage, DatabaseAssemblyStorage>();
        //
        //     services.AddScoped<IAssemblyPlatform, DnlibAssemblyPlatform>();
        //     services.AddScoped<IConfigurationManager, ConfigurationManager>();
        //     services.AddScoped<IXCCompiller, XCCompiler>();
        //
        //
        //     services.AddScoped<IMigrationManager, MigrationManager>();
        //     services.AddSingleton<IPlatformEnvironmentManager, EnvironmentManager>();
        //
        //     //services.AddScoped<ITestEnvironment, TestEnvironment>();
        //     //services.AddScoped<IAdminEnvironment, AdminEnvironment>();
        //     services.AddScoped<IWorkEnvironment, DatabaseTestEnvironment>();
        //
        //     services.AddSingleton<ICacheService, DictionaryCacheService>();
        //
        //
        //     //services.AddTransient<IUserMessageHandler, UserMessageHandler>();
        //     services.AddScoped<IAuthenticationManager, AuthenticationManager>();
        //     services.AddScoped<IDataContextManager, DataContextManager>();
        //     services.AddScoped<IUserManager, UserManager>();
        //
        //
        //     ///SSH
        //     ///
        //
        //
        //     return services.BuildServiceProvider();
        // }
        //
        public static ServiceProvider GetClientService(ITestOutputHelper testOutput)
        {
            IServiceCollection services = new ServiceCollection();


            services.AddSingleton<ClientPlatformContext>();

            services.AddSingleton<IPlatformClient, Network.Client>();

            services.AddTransient(typeof(ILogger<>), typeof(NLogger<>));
            services.AddSingleton(testOutput);
            //services.AddTransient(typeof(ILogger<>), typeof(XUnitLogger<>));

            //services.AddTransient(typeof(ILogger<>), typeof(NLogger<>));
            services.AddSingleton<PlatformAssemblyLoadContext>();
            services.AddSingleton<IClientAssemblyManager, PlatformClientAssemblyManager>();
            services.AddTransient<ITransportClientFactory, TCPTransportClientFactory>();

            services.AddSingleton(factory =>
            {
                var client = factory.GetRequiredService<IPlatformClient>();
                return client.GetService<IAssemblyManagerClientService>();
            });

            return services.BuildServiceProvider();
        }
    }
}