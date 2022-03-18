using System;
using System.IO;
using System.Text;
using Aquila.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Aquila.AspNetCore.Web
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
                return httpcontext.GetOrCreateContext();
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
        public static AqContext GetOrCreateContext(this HttpContext httpctx)
        {
            return AqHttpContext.TryGetFromHttpContext(httpctx) ?? CreateNewContext(httpctx);
        }

        static AqHttpContext CreateNewContext(this HttpContext httpctx)
        {
            return new AqHttpContext(httpctx, null);
        }
    }
}