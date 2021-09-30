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
using Microsoft.Extensions.Hosting;
using SoapCore;

namespace Aquila.WebServiceCore
{
    public class AquilaWebHost : IWebHost, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private IWebHost _host;


        public AquilaWebHost(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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
                .Configure((b, app) =>
                {
                    b.HostingEnvironment.EnvironmentName = Environments.Development;

                    app.UseRouting();
                    app.UseAquilaPlatform(ct);

                    if (b.HostingEnvironment.IsDevelopment())
                    {
                        app.UseDeveloperExceptionPage();
                    }
                    else
                    {
                        app.UseExceptionHandler("/error");
                        app.UseHsts();
                    }

                    app.UseExceptionHandler(c => c.Run(async context =>
                    {
                        var exception = context.Features
                            .Get<IExceptionHandlerPathFeature>()
                            .Error;
                        var response = new { error = exception.Message };
                        await context.Response.WriteAsJsonAsync(response, cancellationToken: ct);
                    }));

                    //if we not found endpoints response this                    
                    app.Run(context =>
                        context.Response.WriteAsync("Path not found! Go away!", cancellationToken: ct));
                })
                .ConfigureServices(services =>
                {
                    services.AddSoapCore();
                    services.AddExceptionHandler(o => o.AllowStatusCode404Response = false);

                    services.AddAquilaPlatform();

                    // If using Kestrel:
                    services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });
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