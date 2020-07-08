using System;
using System.IO;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Aquila.Core;
using Aquila.Core.Authentication;
using Aquila.Core.Contracts.Environment;
using Aquila.Core.Contracts.Network;
using Aquila.Core.Sessions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SoapCore;

namespace Aquila.WebServiceCore
{
    public interface IStartupService
    {
        void Register(Action<IApplicationBuilder> a);

        void RegisterWebServiceClass<T>() where T : class;

        void Configure(IApplicationBuilder buildedr);

        void ConfigureServices(IServiceCollection sc);
    }

    public class RunnerWebService : IWebHost, IDisposable
    {
        private readonly ILogger<RunnerWebService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IStartupService _startupService;
        private readonly IPlatformEnvironmentManager _mrg;
        private IWebHost _host;


        public RunnerWebService(IServiceProvider serviceProvider, IStartupService startupService,
            IPlatformEnvironmentManager mrg)
        {
            _serviceProvider = serviceProvider;
            _startupService = startupService;
            _mrg = mrg;
            _logger = _serviceProvider.GetRequiredService<ILogger<RunnerWebService>>();
        }

        public void Dispose()
        {
        }

        public void Start()
        {
            _host.Start();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    _startupService.ConfigureServices(services);
                    services.AddSoapCore();
                })
                .Configure(app =>
                {
                    app.UseRouting();

                    app.Use(async (context, next) =>
                    {
                        var env = _mrg.GetEnvironment("Library");
                        var plContext = new PlatformContext(env.CreateSession(new Anonymous()));
                        ContextHelper.SetContext(plContext);
                        await next.Invoke();
                    });

                    _startupService.Configure(app);

                    app.Run(context => context.Response.WriteAsync("Default"));
                })
                .UseKestrel()
                .ConfigureServices(x => x.AddRouting())
                .UseContentRoot(Directory.GetCurrentDirectory());

            _host = builder.Build();

            return _host.RunAsync(cancellationToken);
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _host.StopAsync(cancellationToken);
        }

        public IFeatureCollection ServerFeatures => _host.ServerFeatures;

        public IServiceProvider Services => _serviceProvider;
    }
}