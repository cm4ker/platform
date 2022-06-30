using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Aquila.Core;
using Aquila.Core.Instance;
using Aquila.Logging;
using Aquila.Runtime.Infrastructure.Helpers;
using Aquila.Web;
using Aquila.Web.Swagger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Aquila.AspNetCore.Web
{
    internal sealed class AquilaHandlerMiddleware : IMiddleware
    {
        private readonly AqInstanceManager _instanceManager;
        private readonly AquilaApiHolder _holder;
        private readonly ILogger<AqInstance> _logger;

        private object _updateLock = new object();

        public AquilaHandlerMiddleware(AqInstanceManager instanceManager,
            AquilaApiHolder holder,
            ILogger<AqInstance> logger)
        {
            _instanceManager = instanceManager;
            _holder = holder;
            _logger = logger;

            Init();
        }

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var area = context.GetRouteValue(AquilaWebServerExtensions.AreaKey);

            switch (area)
            {
                case "any": return InvokeAny(context, next);
                case "crud": return InvokeCrud(context);
                case "migrate": return InvokeMigrate(context);
                case "deploy": return InvokeDeploy(context);
                case "metadata": return InvokeGetMetadata(context);
                case "user": return InvokeGetCurrentUser(context);
                case "endpoints": return InvokeEndpoints(context);
                case "view": return InvokeView(context, next);


                //in that case we not handle this and just invoke next delegate
                default: return next(context);
            }
        }


        private void Init()
        {
            foreach (var instance in _instanceManager.GetInstances())
            {
                InitInstance(instance);
            }
        }

        private void InitInstance(AqInstance instance)
        {
            lock (_updateLock)
            {
                if (instance.BLAssembly is null)
                {
                    _logger.Info("[Platform] Instance {0} haven't assembly", instance.Name);
                    return;
                }

                if (instance.PendingChanges)
                {
                    _logger.Warn("[Platform] Instance {0} has pending changes. Invoke /api/[instance]/migrate",
                        instance.Name);
                }

                var instanceName = instance.Name.ToLower();
                _holder.UnregisterInstance(instanceName);

                var crudMethods = instance.BLAssembly.GetCrudMethods().ToList();
                var endpointMethods = instance.BLAssembly.GetEndpoints();

                _logger.Info("[Web service] Load {0} delegates", crudMethods.Count);

                foreach (var item in crudMethods)
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

                    var operationType = item.attr.Kind switch
                    {
                        HttpMethodKind.Get => "GET",
                        HttpMethodKind.Post => "PUT",
                        HttpMethodKind.Delete => "DELETE",
                        HttpMethodKind.Create => "GET",
                        HttpMethodKind.List => "GET",
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    _holder.AddCrud(instanceName, item.attr.ObjectName, methodName, operationType, method);
                }

                foreach (var item in endpointMethods)
                {
                    _holder.AddEndpoint(instanceName, item.m.Name.ToLower(), item.m);
                }
            }
        }

        #region Handlers

        private async Task GetHandler(AqContext aqContext, HttpContext context, MethodInfo methodInfo)
        {
            var id = context.GetRouteValue("id")?.ToString() ?? Guid.Empty.ToString();

            try
            {
                var obj = methodInfo.Invoke(null, new object[] { aqContext, Guid.Parse(id) });
                if (obj is null)
                    await context.Response.WriteAsync("null");
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
            InitInstance(instance);

            return Task.CompletedTask;
        }

        public async Task InvokeCrud(HttpContext context)
        {
            var instanceName = (string)context.GetRouteValue("instance");
            var objectName = (string)context.GetRouteValue("object");
            var method = (string)context.GetRouteValue("method");
            var operationType = context.Request.Method;

            var instance = _instanceManager.GetInstance(instanceName);

            var aqctx = new AqHttpContext(context, instance);

            if (_holder.TryGetCrud(instanceName, objectName, method, operationType, out var mi))
            {
                switch (method)
                {
                    case "get":
                        await GetHandler(aqctx, context, mi);
                        break;
                    case "post":
                        await PostHandler(aqctx, context, mi);
                        break;
                    case "delete":
                        await GetHandler(aqctx, context, mi);
                        break;
                }
            }
        }

        private async Task InvokeGetSessions(HttpContext context)
        {
            var instanceName = context.GetRouteValue("instance")?.ToString();
            var instance = _instanceManager.GetInstance(instanceName);

            if (instance is null) return;

            //await context.Response.WriteAsJsonAsync(instance.Sessions);
        }

        private async Task GetInstances(HttpContext context)
        {
            var list = _instanceManager.GetInstances().Select(x => new { x.Name });

            await context.Response.WriteAsJsonAsync(list);
        }

        private async Task InvokeGetCurrentUser(HttpContext context)
        {
            var instanceName = context.GetRouteValue("instance")?.ToString();
            var instance = _instanceManager.GetInstance(instanceName);

            var aqctx = new AqHttpContext(context, instance);

            var result = aqctx.User;

            foreach (var role in aqctx.Roles)
            {
                result += "\n" + role;
            }

            await context.Response.WriteAsJsonAsync(result);
        }

        private async Task InvokeGetMetadata(HttpContext context)
        {
            var instanceName = context.GetRouteValue("instance")?.ToString();
            var instance = _instanceManager.GetInstance(instanceName);

            if (instance is null) return;

            var aqctx = new AqContext(instance);

            var md = aqctx.DataRuntimeContext.Metadata.GetMetadata();

            var list = md.EntityMetadata.ToList();

            await context.Response.WriteAsJsonAsync(list);
        }

        private async Task InvokeEndpoints(HttpContext context)
        {
            var instanceName = context.GetRouteValue("instance")?.ToString();
            var method = context.GetRouteValue("method")?.ToString();

            var instance = _instanceManager.GetInstance(instanceName);

            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var aqctx = new AqHttpContext(context, instance);

            var parametersStream = context.Request.Body;

            if (_holder.TryGetEndpoint(instanceName, method, out var m))
            {
                StreamReader sr = new StreamReader(parametersStream);

                var json = await sr.ReadToEndAsync();

                var methodParams = m.GetParameters();
                var args = new object[methodParams.Length];
                args[0] = aqctx;

                if (!string.IsNullOrEmpty(json))
                {
                    var jobj = JObject.Parse(json);
                    var jsonParameters = jobj.Properties().ToArray();

                    foreach (var jsonParam in jsonParameters)
                    {
                        var realParam = methodParams.FirstOrDefault(x => x.Name == jsonParam.Name);

                        if (realParam != null)
                            args[realParam.Position] =
                                JsonConvert.DeserializeObject(jsonParam.Value.ToString(), realParam.ParameterType);
                    }
                }

                var obj = m.Invoke(null, args);
                if (obj is null)
                    await context.Response.WriteAsync("null");
                else
                    await context.Response.WriteAsJsonAsync(obj, obj.GetType());
            }
        }

        private async Task InvokeAny(HttpContext context, RequestDelegate next)
        {
            await next(context);
        }

        private async Task InvokeView(HttpContext context, RequestDelegate next)
        {
            var instanceName = context.GetRouteValue("instance")?.ToString();
            var instance = _instanceManager.GetInstance(instanceName);

            if (instance is null)
            {
                await next(context);
                return;
            }

            context.Items["AquilaAssemblies"] = new[] { instance.BLAssembly, typeof(AquilaHandlerMiddleware).Assembly };
            await next(context);
        }

        #endregion
    }
}