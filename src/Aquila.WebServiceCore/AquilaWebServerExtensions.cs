using System;
using Aquila.AspNetCore.Web;
using Aquila.Core.Authentication;
using Aquila.Core.CacheService;
using Aquila.Core.Contracts.Authentication;
using Aquila.Core.Contracts.Network;
using Aquila.Core.Instance;
using Aquila.Core.Network;
using Aquila.Core.Serialisers;
using Aquila.Core.Settings;
using Aquila.Core.Utilities;
using Aquila.Data;
using Aquila.Logging;
using Aquila.Migrations;
using Aquila.Networking;
using Aquila.Shell;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Aquila.WebServiceCore
{
    public static class AquilaWebServerExtensions
    {
        public static IApplicationBuilder UseAquila(this IApplicationBuilder builder)
        {
            builder.MapMiddleware<AquilaHandlerMiddleware>("default", "migrate",
                "api/{instance}/migrate");

            return builder.MapMiddleware<AquilaHandlerMiddleware>("default", "crud",
                "api/{instance}/{object}/{method}/{id?}");
        }

        private static IApplicationBuilder MapMiddleware<T>(this IApplicationBuilder builder, string name,
            string areaName, string template)
        {
            var routeBuilder = new RouteBuilder(builder);

            var defaultsDictionary = new RouteValueDictionary() { { "area", areaName } };
            var constraintsDictionary = new RouteValueDictionary() { { "area", areaName } };

            var nested = builder.New();
            nested.UseMiddleware<AquilaHandlerMiddleware>();

            var route = new Route(
                new RouteHandler(nested.Build()),
                name,
                template,
                defaults: defaultsDictionary,
                constraints: constraintsDictionary,
                dataTokens: null,
                inlineConstraintResolver: routeBuilder.ServiceProvider.GetRequiredService<IInlineConstraintResolver>());

            routeBuilder.Routes.Add(route);
            builder.UseRouter(routeBuilder.Build());

            return builder;
        }

        public static IServiceCollection AddAquila(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddScoped<DataContextManager>();
            services.AddScoped<UserManager>();

            services.AddSingleton<AqInstanceManager, AqInstanceManager>();
            services.AddScoped<AqInstance>();
            services.AddScoped<MigrationManager>();

            services.AddSingleton<AqInstanceManager, AqInstanceManager>();
            services.AddSingleton<ISettingsStorage, FileSettingsStorage>();

            services.AddTransient<IConnectionManager, ConnectionManager>();
            services.AddTransient(typeof(ILogger<>), typeof(SimpleConsoleLogger<>));
            services.AddTransient<IDatabaseNetworkListener, TCPListener>();

            services.AddScoped<IInvokeService, InvokeService>();
            services.AddTransient<INetworkListener, TCPListener>();
            services.AddTransient<ITerminalNetworkListener, SSHListener>();
            services.AddTransient<IChannel, Channel>();
            services.AddSingleton<IAccessPoint, UserAccessPoint>();
            services.AddSingleton<ITaskManager, TaskManager>();
            services.AddTransient<IMessagePackager, SimpleMessagePackager>();
            services.AddTransient<ISerializer, ApexSerializer>();
            services.AddTransient<UserConnectionFactory>();
            services.AddTransient<ServerConnectionFactory>();
            services.AddTransient<IChannelFactory, ChannelFactory>();

            services.AddSingleton<ICacheService, DictionaryCacheService>();

            services.AddScoped<IAuthenticationManager, AuthenticationManager>();

            services.AddSingleton<AquilaHandlerMiddleware>();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            ContextExtensions.CurrentContextProvider = () => HttpContextExtension.GetOrCreateContext();


            return services;
        }
    }
}