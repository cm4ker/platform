using System;
using System.IO;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Aquila.Core.Contracts.Network;
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
    public class RunnerWebService : IWebHost, IDisposable
    {
        private readonly ILogger<RunnerWebService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IStartupService _startupService;
        private IWebHost _host;


        public RunnerWebService(IServiceProvider serviceProvider, IStartupService startupService)
        {
            _serviceProvider = serviceProvider;
            _startupService = startupService;
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
                    
                    services.TryAddSingleton<SampleService>();
                    services.AddSoapCore();
                })
                .Configure(app =>
                {
                    app.UseRouting();

                    app.ApplicationServices.GetService(typeof(IStartupService));

                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/hello/{name}/{name2}", async context =>
                        {
                            var name = context.Request.RouteValues["name"];
                            var name2 = context.Request.RouteValues["name2"];

                            await context.Response.WriteAsync($"Hello {name} {name2}!");
                        });
                    });

                    app.UseSoapEndpoint<SampleService>("/Service.svc", new BasicHttpBinding());
                    app.UseSoapEndpoint<SampleService>("/Service.asmx", new BasicHttpBinding(),
                        SoapSerializer.XmlSerializer);

                    _startupService.Configure(app);

                    app.Run(context => context.Response.WriteAsync("Default"));
                })
                .UseKestrel()
                //.UseStartup<Startup>()
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