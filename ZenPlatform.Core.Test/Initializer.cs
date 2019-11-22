using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using ZenPlatform.Core.Settings;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Data;
using ZenPlatform.Core.CacheService;
using ZenPlatform.Core.Environment;
using ZenPlatform.Configuration.Data.Contracts;
using ZenPlatform.Core.Configuration;
using ZenPlatform.Core.Assemblies;
using ZenPlatform.Core.ClientServices;
using ZenPlatform.Core.Network;
using ZenPlatform.Core.Serialisers;
using ZenPlatform.Core.Logging;
using ZenPlatform.Core.Tools;
using ZenPlatform.Compiler.Platform;
using ZenPlatform.Configuration;
using ZenPlatform.Core.Test.Configuration;
using ZenPlatform.Core.Test.Environment;
using ZenPlatform.Core.Assemlies;
using ZenPlatform.Core.Environment.Contracts;
using ZenPlatform.Core.Network.Contracts;
using ZenPlatform.Core.Test.Assemblies;
using ZenPlatform.Networking;
using ZenPlatform.Shell;
using Xunit.Abstractions;
using ZenPlatform.Shell.Terminal;
using McMaster.Extensions.CommandLineUtils;
using ZenPlatform.Cli;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Shell.Contracts;

namespace ZenPlatform.Core.Test
{
    public static class Initializer
    {
        

        public static ServiceProvider GetServerService(ITestOutputHelper testOutput)
        {
            IServiceCollection services = new ServiceCollection();

            services.AddTransient<IConnectionManager, ConnectionManager>();
            services.AddSingleton(testOutput);
            //services.AddTransient(typeof(ILogger<>), typeof(XUnitLogger<>));
            services.AddTransient(typeof(ILogger<>), typeof(NLogger<>));
            services.AddScoped<IInvokeService, InvokeService>();

            services.AddTransient<ITerminalNetworkListener, SSHListener>();
            services.AddTransient<IDatabaseNetworkListener, TCPListener>();

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
            services.AddSingleton<IConfigurationManipulator, XCConfManipulator>();
            services.AddScoped<IAssemblyManager, AssemblyManager>();
            services.AddSingleton<ISettingsStorage, TestSettingsStorage>();
            services.AddSingleton<IXCConfigurationStorage, XCTestStorage>();
            services.AddSingleton<IAssemblyStorage, TestAssemblyStorage>();

            services.AddScoped<IConfigurationManager, ConfigurationManager>();
            services.AddScoped<IXCCompiller, XCCompiler>();


            //services.AddSingleton<ITestProxyService, TestProxyService>();
            services.AddSingleton<IPlatformEnvironmentManager, EnvironmentManager>();

            //services.AddScoped<ITestEnvironment, TestEnvironment>();
            //services.AddScoped<IAdminEnvironment, AdminEnvironment>();
            services.AddScoped<IWorkEnvironment, TestEnvironment>();

            services.AddSingleton<ICacheService, DictionaryCacheService>();


            //services.AddTransient<IUserMessageHandler, UserMessageHandler>();
            services.AddScoped<IAuthenticationManager, AuthenticationManager>();
            services.AddScoped<IDataContextManager, DataContextManager>();
            services.AddScoped<IUserManager, UserManager>();


            ///SSH
            ///
            services.AddScoped<ITerminalSession, TerminalSession>();
            services.AddScoped<ITerminalApplication, CommandApplication>();
            services.AddScoped<ITerminal, VirtualTerminal>();
            services.AddScoped<IConsole, TerminalConsole>();
            services.AddScoped<ICommandLineInterface, McMasterCommandLineInterface>();
            

            return services.BuildServiceProvider();
        }

        public static ServiceProvider GetClientService(ITestOutputHelper testOutput)
        {
            IServiceCollection services = new ServiceCollection();


            services.AddSingleton<ClientPlatformContext>();
            
            services.AddSingleton<IPlatformClient, Network.Client>();

            //services.AddTransient(typeof(ILogger<>), typeof(NLogger<>));
            services.AddSingleton(testOutput);
            //services.AddTransient(typeof(ILogger<>), typeof(XUnitLogger<>));

            services.AddTransient(typeof(ILogger<>), typeof(NLogger<>));
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