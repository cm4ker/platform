using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Aquila.Core;
using Aquila.Core.Instance;
using Aquila.Logging;
using Aquila.Runtime.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Aquila.AspNetCore.Web
{
    internal sealed class AquilaHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AqInstanceManager _instanceManager;
        private readonly ILogger<AqInstance> _logger;

        private Dictionary<(string instance, string objectName, string method), MethodInfo> _handlers = new();


        public AquilaHandlerMiddleware(RequestDelegate next, AqInstanceManager instanceManager,
            ILogger<AqInstance> logger)
        {
            _next = next;
            _instanceManager = instanceManager;
            _logger = logger;

            foreach (var insatnce in _instanceManager.GetInstances())
            {
                if (insatnce.BLAssembly is null)
                {
                    logger.Info("[Platform] Instance {0} haven't assembly", insatnce.Name);
                    return;
                }

                if (insatnce.PendingChanges)
                {
                    logger.Info("[Platform] Instance {0} has pending changes. Invoke /api/[instance]/migrate",
                        insatnce.Name);
                }

                var methods = insatnce.BLAssembly.GetCrudMethods().ToList();
                logger.Info("[Web service] Load {0} delegates", methods.Count);

                foreach (var item in methods)
                {
                    var method = item.m;

                    if (!method.IsStatic || !method.IsPublic
                                         || method.GetParameters().FirstOrDefault()?.ParameterType !=
                                         typeof(AqContext))
                        throw new Exception($"Method {method.Name} marked as a CRUD but not consistent");

                    var methodName = item.attr.Kind switch
                    {
                        HttpMethodKind.Get => "get",
                        HttpMethodKind.Post => "post",
                        HttpMethodKind.Delete => "delete",
                        HttpMethodKind.Create => "create",
                        HttpMethodKind.List => "list",
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    _handlers.Add((insatnce.Name.ToLower(), item.attr.ObjectName, methodName), method);
                }
            }
        }

        private async Task GetHandler(AqContext aqContext, HttpContext context, MethodInfo methodInfo)
        {
            var id = context.GetRouteValue("id")?.ToString() ?? Guid.Empty.ToString();

            try
            {
                var obj = methodInfo.Invoke(null, new object[] { aqContext, Guid.Parse(id) });
                if (obj is null)
                    await context.Response.WriteAsync("The object is null");
                else
                    await context.Response.WriteAsJsonAsync(obj, obj.GetType());
            }
            catch (Exception ex)
            {
                context.Response.ContentLength = ex.ToString().Length;
                await context.Response.WriteAsync(ex.ToString());
            }
        }

        private async Task PostHandler(AqContext aqContext, HttpContext context, MethodInfo methodInfo)
        {
            try
            {
                var data = await JsonSerializer.DeserializeAsync(context.Request.Body,
                    methodInfo.GetParameters()[1].ParameterType,
                    options: new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                methodInfo.Invoke(null, new object[] { aqContext, data });
            }
            catch (Exception ex)
            {
                context.Response.ContentLength = ex.ToString().Length;
                await context.Response.WriteAsync(ex.ToString());
            }
        }


        public Task InvokeAsync(HttpContext context)
        {
            var area = context.GetRouteValue("area");

            switch (area)
            {
                case "crud": return InvokeCrud(context);
                case "migrate": return InvokeMigrate(context);
                case "deploy": return InvokeDeploy(context);

                //in that case we not handle this and just invoke next delegate
                default: return _next(context);
            }
        }

        private Task InvokeDeploy(HttpContext context)
        {
            var instanceName = context.GetRouteValue("instance")?.ToString();
            var instance = _instanceManager.GetInstance(instanceName);

            if (instance is null) return Task.CompletedTask;

            try
            {
                var zipStream = context.Request.Body;
                instance.Deploy(zipStream);
            }
            catch (Exception ex)
            {
                //Do smth package is corrupted
                throw;
            }

            return Task.CompletedTask;
        }

        private Task InvokeMigrate(HttpContext context)
        {
            var instanceName = context.GetRouteValue("instance")?.ToString();
            var instance = _instanceManager.GetInstance(instanceName);

            if (instance is null) return Task.CompletedTask;

            instance.Migrate();
            return Task.CompletedTask;
        }

        public Task InvokeCrud(HttpContext context)
        {
            var instanceName = (string)context.GetRouteValue("instance");
            var objectName = (string)context.GetRouteValue("object");
            var method = (string)context.GetRouteValue("method");

            var instance = _instanceManager.GetInstance(instanceName);

            var aqctx = new AqHttpContext(context, instance);

            if (_handlers.TryGetValue((instanceName, objectName, method), out var mi))
            {
                switch (method)
                {
                    case "get": return GetHandler(aqctx, context, mi);
                    case "post": return PostHandler(aqctx, context, mi);
                    case "delete": return GetHandler(aqctx, context, mi);
                }
            }

            return Task.CompletedTask;
        }


        private async Task InvokeGetSessions(HttpContext context)
        {
            var instanceName = context.GetRouteValue("instance")?.ToString();
            var instance = _instanceManager.GetInstance(instanceName);

            if (instance is null) return;

            await context.Response.WriteAsJsonAsync(instance.Sessions);
        }

        private async Task GetInstances(HttpContext context)
        {
            var list = _instanceManager.GetInstances().Select(x => new { x.Name });

            await context.Response.WriteAsJsonAsync(list);
        }

        private async Task GetInstanceMetadata(HttpContext context)
        {
            var instanceName = context.GetRouteValue("instance")?.ToString();
            var instance = _instanceManager.GetInstance(instanceName);

            if (instance is null) return;

            var plContext = new AqContext(instance);

            var md = plContext.DataRuntimeContext.Metadata.GetMetadata();

            var list = md.Metadata.ToList();

            await context.Response.WriteAsJsonAsync(list);
        }
    }
}