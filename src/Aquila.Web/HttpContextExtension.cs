using System;
using System.IO;
using Aquila.AspNetCore.Web;
using Aquila.Core;
using Aquila.Core.Instance;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Aquila.Web
{
    /// <summary>
    /// Provides methods for <see cref="HttpContext"/>.
    /// </summary>
    public static class HttpContextExtension
    {
        /// <summary>
        /// Gets default root path.
        /// </summary>
        internal static string GetDefaultRootPath(this IHostingEnvironment hostingEnv)
        {
            return hostingEnv.WebRootPath ?? hostingEnv.ContentRootPath ?? Directory.GetCurrentDirectory();
        }

        /// <summary>
        /// Gets <see cref="HttpContext"/> associated with given Web <see cref="Context"/>.
        /// </summary>
        /// <exception cref="ArgumentException">Given context is not a web context.</exception>
        public static HttpContext /*!*/ GetHttpContext(this AqContext context)
        {
            if (context is AqHttpContext reqcontext)
            {
                return reqcontext.HttpContext;
            }
            else
            {
                throw new ArgumentException(nameof(context));
            }
        }

        /// <summary>
        /// Gets context associated with current <see cref="HttpContext"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown in case <see cref="IHttpContextAccessor"/> is not registered
        /// or a current <see cref="HttpContext"/> cannot be obtained.</exception>
        public static AqContext GetOrCreateContext()
        {
            if (s_HttpContextAccessor == null)
            {
                s_HttpContextAccessor = new HttpContextAccessor();
            }

            var httpcontext = s_HttpContextAccessor.HttpContext;
            // uses AsyncLocal to maintain value within ExecutionContext, set by ASP.NET Core framework

            if (httpcontext != null)
            {
                return httpcontext.TryGetOrCreateContext() ?? throw new InvalidOperationException();
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        static HttpContextAccessor s_HttpContextAccessor;

        /// <summary>
        /// Gets existing context associated with given <see cref="HttpContext"/> or creates new one with default settings.
        /// </summary>
        public static AqContext TryGetOrCreateContext(this HttpContext httpctx)
        {
            var manager = httpctx.RequestServices.GetService<AqInstanceManager>();
            return AqHttpContext.TryGetFromHttpContext(httpctx)
                   ?? httpctx.TryCreateContext(manager);
        }

        internal static AqHttpContext CreateContext(this HttpContext httpctx, AqInstance instance)
        {
            return new AqHttpContext(httpctx, instance);
        }

        [CanBeNull]
        static AqHttpContext TryCreateContext(this HttpContext context, AqInstanceManager manager)
        {
            if (manager == null || context == null)
                return null;
            
            if (context.TryGetInstanceName(out var name)
                && manager.TryGetInstance(name, out var instance))
            {
                return context.CreateContext(instance);
            }

            return null;
        }

        public static bool TryGetInstanceName(this HttpContext ctx, out string value)
        {
            return ctx.TryGetRouteValue(AquilaWellKnowWebNames.RouteInstanceName, out value);
        }

        public static bool TryGetObjectName(this HttpContext ctx, out string value)
        {
            return ctx.TryGetRouteValue(AquilaWellKnowWebNames.RouteObjectName, out value);
        }

        public static bool TryGetMethodName(this HttpContext ctx, out string value)
        {
            return ctx.TryGetRouteValue(AquilaWellKnowWebNames.RouteMethodName, out value);
        }

        public static bool TryGetRouteValue(this HttpContext ctx, string routeName, out string value)
        {
            value = ctx.GetRouteStringValue(routeName);
            return value != null;
        }

        private static string GetRouteStringValue(this HttpContext ctx, string routeName)
        {
            return ctx.GetRouteValue(routeName) switch
            {
                null => null,
                string s => s,
                _ => null
            };
        }
    }
}