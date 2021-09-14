using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Aquila.Core;
using Aquila.Core.Authentication;
using Aquila.Core.Contracts.Instance;
using Aquila.Core.Sessions;
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

                            foreach (var item in methods.Where(x => x.attr.Kind == HttpMethodKind.Get))
                            {
                                var method = item.m;

                                if (!method.IsStatic || !method.IsPublic
                                                     || method.GetParameters().FirstOrDefault()?.ParameterType !=
                                                     typeof(AqContext))
                                    throw new Exception($"Method {method.Name} marked as a CRUD but not consistent");


                                //NOTE: route in assembly starts from /
                                var route = $"api/{{instance}}{item.attr.Route}";
                                _logger.Info($"Add route for get = {route.Replace('{', '(').Replace('}', ')')}");

                                x.MapGet(route, context => GetHandler(context, method, cancellationToken));
                            }

                            foreach (var item in methods.Where(x => x.attr.Kind == HttpMethodKind.Post))
                            {
                                var method = item.m;

                                if (!method.IsStatic || !method.IsPublic
                                                     || method.GetParameters().FirstOrDefault()?.ParameterType !=
                                                     typeof(AqContext))
                                    throw new Exception($"Method {method.Name} marked as a CRUD but not consistent");


                                //NOTE: route in assembly starts from /
                                var route = $"api/{{instance}}{item.attr.Route}";
                                _logger.Info($"Add route for get = {route.Replace('{', '(').Replace('}', ')')}");

                                x.MapPost(route, context => PostHandler(context, method, cancellationToken));
                            }
                        });
                    }


                    app.UseEndpoints(x =>
                    {
                        x.MapGet("api/{instance}/getMetadata",
                            async context => { await GetInstanceMetadata(context, cancellationToken); });
                        x.MapGet("api/admin/instances",
                            async context => { await GetInstances(context, cancellationToken); });
                        x.MapGet("api/admin/{instance}/sessions",
                            async context => { await GetSessions(context, cancellationToken); });
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

        private async Task PostHandler(HttpContext context, MethodInfo method, CancellationToken cancellationToken)
        {
            var instanceName = context.GetRouteData().Values["instance"]?.ToString();
            var instance = _mrg.GetInstance(instanceName);
            if (instance is null) return;

            try
            {
                var session = instance.CreateSession(new Anonymous());

                var data = await JsonSerializer.DeserializeAsync(context.Request.Body,
                    method.GetParameters()[1].ParameterType, cancellationToken: cancellationToken,
                    options: new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                method.Invoke(null, new object[] { new AqContext(session), data });
            }
            catch (Exception ex)
            {
                context.Response.ContentLength = ex.ToString().Length;
                await context.Response.WriteAsync(ex.ToString(), cancellationToken: cancellationToken);
            }
        }

        private async Task GetSessions(HttpContext context, CancellationToken token)
        {
            var instanceName = context.GetRouteData().Values["instance"]?.ToString();

            var env = _mrg.GetInstance(instanceName);

            if (env is null) return;

            await context.Response.WriteAsJsonAsync(env.Sessions, token);
        }

        private async Task GetInstances(HttpContext context, CancellationToken token)
        {
            var list = _mrg.GetInstances().Select(x => new { x.Name });

            await context.Response.WriteAsJsonAsync(list, cancellationToken: token);
        }

        private async Task GetInstanceMetadata(HttpContext context, CancellationToken token)
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

            await context.Response.WriteAsJsonAsync(list, cancellationToken: token);
        }

        private async Task GetHandler(HttpContext context, MethodInfo methodInfo, CancellationToken token)
        {
            var instanceName = context.GetRouteData().Values["instance"]?.ToString();

            var instance = _mrg.GetInstance(instanceName);

            if (instance is null) return;

            var id = context.GetRouteData().Values["id"]?.ToString() ?? Guid.Empty.ToString();

            try
            {
                var session = instance.CreateSession(new Anonymous());

                var obj = methodInfo.Invoke(null, new object[] { new AqContext(session), Guid.Parse(id) });
                if (obj is null)
                    await context.Response.WriteAsync("The object is null", cancellationToken: token);
                else
                    await context.Response.WriteAsJsonAsync(obj, obj.GetType(), cancellationToken: token);
            }
            catch (Exception ex)
            {
                context.Response.ContentLength = ex.ToString().Length;
                await context.Response.WriteAsync(ex.ToString(), cancellationToken: token);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _host.StopAsync(cancellationToken);
        }

        public IFeatureCollection ServerFeatures => _host.ServerFeatures;

        public IServiceProvider Services => _serviceProvider;
    }
}