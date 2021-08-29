using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aquila.Core;
using Aquila.Core.Authentication;
using Aquila.Core.Contracts.Instance;
using Aquila.Core.Contracts.Network;
using Aquila.Core.Sessions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        private readonly IPlatformInstanceManager _mrg;
        private IWebHost _host;


        public RunnerWebService(IServiceProvider serviceProvider, IStartupService startupService,
            IPlatformInstanceManager mrg)
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

                    services.AddExceptionHandler(o => o.AllowStatusCode404Response = false);
                })
                .Configure(app =>
                {
                    app.UseRouting();

                    app.UseEndpoints(x =>
                        x.MapGet("dbName/api/entity/invoice", async context => { await CustomHandler(context); }));

                    _startupService.Configure(app);

                    async Task CustomHandler(HttpContext context)
                    {
                        /*
                         1. Generate crud api
                         
                         Create()                               
                         
                         Get()                                GET       GetList
                         
                         FROM Invoice SELECT *
                         
                         Get(lookup_fields, condition)        GET       advanced GetList
                         
                         FROM Invoice WHERE condition SELECT lookup_fields
                         
                         Get(id)                              GET       GetById
                         
                         Post(object)                         POST      Update object or store
                         
                         Delete(id)                           DELETE    Delete object
                                                  
                         */

                        var env = _mrg.GetInstance("Library");
                        var plContext = new AqContext(env.CreateSession(new Anonymous()));

                        var md = plContext.DataRuntimeContext.GetMetadata();

                        var list = md.Metadata.ToList();

                        await context.Response.WriteAsJsonAsync(list, cancellationToken);
                    }

                    //if we not found endpoints response this                    
                    app.Run(context =>
                        context.Response.WriteAsync("Path not found! Go away!", cancellationToken: cancellationToken));
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