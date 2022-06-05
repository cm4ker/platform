using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SoapCore;

namespace Aquila.Web
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
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("hosting.json", optional: false, reloadOnChange: true)
                .Build();

            var builder = new WebHostBuilder()
                
                .UseConfiguration(config)
                .ConfigureServices(services =>
                {
                    services.AddDistributedMemoryCache();
                    services.AddSession();
                    services.AddAquila();
                })
                .ConfigureAppConfiguration((ctx, cb) =>
                {
                    if (ctx.HostingEnvironment.IsDevelopment())
                    {
                        StaticWebAssetsLoader.UseStaticWebAssets(ctx.HostingEnvironment, ctx.Configuration);
                    }
                })
                .Configure((b, app) =>
                {
                    b.HostingEnvironment.EnvironmentName = Environments.Development;

                    if (b.HostingEnvironment.IsDevelopment())
                    {
                        app.UseDeveloperExceptionPage();
                    }
                    else
                    {
                        app.UseExceptionHandler("/error");
                        app.UseHsts();
                    }


                    app.UseAquila(b);
                    //if we not found endpoints response this                    
                    app.Run(context =>
                        context.Response.WriteAsync("Path not found! Go away!", cancellationToken: ct));
                })
                .UseKestrel()
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