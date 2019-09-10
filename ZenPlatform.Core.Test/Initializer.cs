﻿using System;
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
using ZenPlatform.Core.Test.Assemblies;

namespace ZenPlatform.Core.Test
{
    public static class Initializer
    {
        public static ServiceProvider GetServerService()
        {
            IServiceCollection services = new ServiceCollection();

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



            services.AddScoped<IAssemblyManager, AssemblyManager>();
            services.AddSingleton<ISettingsStorage, TestSettingsStorage>();
            services.AddSingleton<IXCConfigurationStorage, XCTestStorage>();
            services.AddSingleton<IAssemblyStorage, TestAssemblyStorage>();


            services.AddScoped<IConfigurationManager, ConfigurationManager>();
            services.AddScoped<IXCCompiller, XCCompiller>();


            //services.AddSingleton<ITestProxyService, TestProxyService>();
            services.AddSingleton<IEnvironmentManager, EnvironmentManager>();

            //services.AddScoped<ITestEnvironment, TestEnvironment>();
            services.AddScoped<IAdminEnvironment, AdminEnvironment>();
            services.AddScoped<IWorkEnvironment, TestEnvironment>();

            services.AddSingleton<ICacheService, DictionaryCacheService>();


            //services.AddTransient<IUserMessageHandler, UserMessageHandler>();
            services.AddScoped<IAuthenticationManager, AuthenticationManager>();
            services.AddScoped<IDataContextManager, DataContextManager>();
            services.AddScoped<IUserManager, UserManager>();

            return services.BuildServiceProvider();

        }

        public static ServiceProvider GetClientService()
        {
            IServiceCollection services = new ServiceCollection();


            services.AddSingleton<PlatformClient>();
            services.AddSingleton<IClient, Client>();
            services.AddTransient(typeof(ILogger<>), typeof(NLogger<>));
            services.AddSingleton<PlatformAssemblyLoadContext>();
            services.AddSingleton<IClientAssemblyManager, ClientAssemblyManager>();



            services.AddSingleton(factory =>
            {
                var client = factory.GetRequiredService<IClient>();
                return client.GetService<IAssemblyManagerClientService>();
            });

            return services.BuildServiceProvider();
        }
    }
}
