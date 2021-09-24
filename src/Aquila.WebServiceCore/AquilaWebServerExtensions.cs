using System;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Aquila.Core;
using Aquila.Core.Authentication;
using Aquila.Core.Contracts.Instance;
using Aquila.Logging;
using Aquila.Runtime.Infrastructure.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Aquila.WebServiceCore
{
    public static class AquilaWebServerExtensions
    {
        public static void UseAquilaPlatform(this IApplicationBuilder app, IPlatformInstanceManager mrg,
            ILogger _logger,
            CancellationToken ct = default)
        {
            var instances = mrg.GetInstances();

            foreach (var inst in instances)
            {
                var methods = inst.BLAssembly.GetLoadMethod().ToList();
                _logger.Info("[Web service] Load {0} delegates", methods.Count);

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

                        x.MapGet(route, context => GetHandler(context, method, mrg, ct));
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

                        x.MapPost(route, context => PostHandler(context, method, mrg, ct));
                    }
                });
            }


            app.UseEndpoints(x =>
            {
                x.MapGet("api/{instance}/getMetadata",
                    async context => { await GetInstanceMetadata(context, mrg, ct); });
                x.MapPost("api/{instance}/deploy",
                    async context => { await DeployConfig(context, mrg, ct); });
                x.MapGet("api/admin/instances",
                    async context => { await GetInstances(context, mrg, ct); });
                x.MapGet("api/admin/{instance}/sessions",
                    async context => { await GetSessions(context, mrg, ct); });
            });
        }

        private static async Task DeployConfig(HttpContext context, IPlatformInstanceManager mrg, CancellationToken ct)
        {
            var instanceName = context.GetRouteData().Values["instance"]?.ToString();

            var instance = mrg.GetInstance(instanceName);

            if (instance is null) return;

            var zipStream = context.Request.Body;

            ZipArchive s = new ZipArchive(zipStream, ZipArchiveMode.Read);

            var e = s.Entries.ToList();
        }


        private static async Task PostHandler(HttpContext context, MethodInfo method, IPlatformInstanceManager mrg,
            CancellationToken cancellationToken)
        {
            var instanceName = context.GetRouteData().Values["instance"]?.ToString();
            var instance = mrg.GetInstance(instanceName);
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

        private static async Task GetSessions(HttpContext context, IPlatformInstanceManager mrg,
            CancellationToken token)
        {
            var instanceName = context.GetRouteData().Values["instance"]?.ToString();

            var env = mrg.GetInstance(instanceName);

            if (env is null) return;

            await context.Response.WriteAsJsonAsync(env.Sessions, token);
        }

        private static async Task GetInstances(HttpContext context, IPlatformInstanceManager mrg,
            CancellationToken token)
        {
            var list = mrg.GetInstances().Select(x => new { x.Name });

            await context.Response.WriteAsJsonAsync(list, cancellationToken: token);
        }

        private static async Task GetInstanceMetadata(HttpContext context, IPlatformInstanceManager mrg,
            CancellationToken token)
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

            var instance = mrg.GetInstance(instanceName);

            if (instance is null) return;

            var plContext = new AqContext(instance.CreateSession(new Anonymous()));

            var md = plContext.DataRuntimeContext.Metadata.GetMetadata();

            var list = md.Metadata.ToList();

            await context.Response.WriteAsJsonAsync(list, cancellationToken: token);
        }

        private static async Task GetHandler(HttpContext context, MethodInfo methodInfo, IPlatformInstanceManager mrg,
            CancellationToken token)
        {
            var instanceName = context.GetRouteData().Values["instance"]?.ToString();

            var instance = mrg.GetInstance(instanceName);

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
    }
}