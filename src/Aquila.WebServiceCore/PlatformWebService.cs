using System;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Aquila.Core.Contracts.Instance;
using Aquila.Core.Instance;
using Aquila.Core.Sessions;
using Aquila.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using SoapCore;

namespace Aquila.WebServiceCore
{
    public class PlatformWebService : IWebHost, IDisposable
    {
        private readonly ILogger<PlatformWebService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IStartupService _startupService;
        private readonly IPlatformInstanceManager _mrg;
        private IWebHost _host;


        public PlatformWebService(IServiceProvider serviceProvider, IStartupService startupService,
            IPlatformInstanceManager mrg, ILogger<PlatformWebService> looger)
        {
            _serviceProvider = serviceProvider;
            _startupService = startupService;
            _mrg = mrg;
            _logger = looger;
        }

        public void Dispose()
        {
        }

        public void Start()
        {
            _host.Start();
        }

        public Task StartAsync(CancellationToken ct)
        {
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    _startupService.ConfigureServices(services);
                    services.AddSoapCore();
                    services.AddExceptionHandler(o => o.AllowStatusCode404Response = false);

                    // If using Kestrel:
                    services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });
                })
                .Configure((b, app) =>
                {
                    b.HostingEnvironment.EnvironmentName = "Development";

                    app.UseRouting();
                    app.UseAquilaPlatform(_mrg, _logger, ct);

                    app.UseExceptionHandler(c => c.Run(async context =>
                    {
                        var exception = context.Features
                            .Get<IExceptionHandlerPathFeature>()
                            .Error;
                        var response = new { error = exception.Message };
                        await context.Response.WriteAsJsonAsync(response, cancellationToken: ct);
                    }));

                    _startupService.Configure(app);

                    //if we not found endpoints response this                    
                    app.Run(context =>
                        context.Response.WriteAsync("Path not found! Go away!", cancellationToken: ct));
                })
                .UseKestrel()
                .ConfigureServices(x => x.AddRouting())
                .UseContentRoot(Directory.GetCurrentDirectory());

            _host = builder.Build();

            return _host.RunAsync(ct);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _host.StopAsync(cancellationToken);
        }

        public IFeatureCollection ServerFeatures => _host.ServerFeatures;

        public IServiceProvider Services => _serviceProvider;
    }
}