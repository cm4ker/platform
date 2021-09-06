using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aquila.Core;
using Aquila.Core.Authentication;
using Aquila.Core.Contracts.Instance;
using Aquila.Logging;
using Aquila.Runtime.Infrastructure.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using SoapCore;

namespace Aquila.WebServiceCore
{
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
                .Configure((b, app) =>
                {
                    b.HostingEnvironment.EnvironmentName = "Development";

                    app.UseRouting();


                    var instances = _mrg.GetInstances();

                    foreach (var inst in instances)
                    {
                        var methods = inst.BLAssembly.GetLoadMethod().ToList();
                        _logger.Info("We catch {0} delegates", methods.Count);


                        app.UseEndpoints(x =>
                        {
                            var index = 0;

                            foreach (var method in methods)
                            {
                                if (!method.IsStatic || !method.IsPublic
                                                     || method.GetParameters().FirstOrDefault()?.ParameterType !=
                                                     typeof(AqContext))
                                    throw new Exception($"Method {method.Name} marked as a CRUD but not consistent");

                                x.MapGet($"api/{{instance}}/entity{index++}/get",
                                    async context =>
                                    {
                                        var obj = method.Invoke(null, new[] { new AqContext(null), });
                                        
                                        await context.Response.WriteAsJsonAsync(obj, obj.GetType(),
                                            cancellationToken: cancellationToken);
                                    });
                            }
                        });
                    }


                    app.UseEndpoints(x =>
                    {
                        x.MapGet("api/{instance}/getMetadata",
                            async context => { await GetInstanceMetadata(context); });
                        x.MapGet("api/admin/instances", async context => { await GetInstances(context); });
                        x.MapGet("api/admin/{instance}/sessions", async context => { await GetSessions(context); });
                    });


                    app.UseExceptionHandler(c => c.Run(async context =>
                    {
                        var exception = context.Features
                            .Get<IExceptionHandlerPathFeature>()
                            .Error;
                        var response = new { error = exception.Message };
                        await context.Response.WriteAsJsonAsync(response);
                    }));

                    _startupService.Configure(app);

                    async Task GetInstanceMetadata(HttpContext context)
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

                        var instanceName = context.GetRouteData().Values["instance"]?.ToString();

                        var instance = _mrg.GetInstance(instanceName);

                        if (instance is null) return;

                        var plContext = new AqContext(instance.CreateSession(new Anonymous()));

                        var md = plContext.DataRuntimeContext.GetMetadata();

                        var list = md.Metadata.ToList();

                        await context.Response.WriteAsJsonAsync(list, cancellationToken);
                    }

                    async Task GetInstances(HttpContext context)
                    {
                        var list = _mrg.GetInstances().Select(x => new { x.Name });

                        await context.Response.WriteAsJsonAsync(list, cancellationToken);
                    }

                    async Task GetSessions(HttpContext context)
                    {
                        var instanceName = context.GetRouteData().Values["instance"]?.ToString();

                        var env = _mrg.GetInstance(instanceName);

                        if (env is null) return;

                        await context.Response.WriteAsJsonAsync(env.Sessions, cancellationToken);
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