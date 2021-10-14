using System;
using Aquila.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;

namespace Aquila.AspNetCore.Web.Session
{
    /// <summary>
    /// Session handler for ASP.NET Core.
    /// </summary>
    sealed class AspNetCoreSessionHandler : AquilaSessionHandler
    {
        public static readonly AquilaSessionHandler Default = new AspNetCoreSessionHandler();

        private AspNetCoreSessionHandler() { }

        static HttpContext GeHttpContext(IHttpAqContext webctx) => ((AqHttpContext)webctx).HttpContext;

        /// <summary>
        /// Gets the session name.
        /// </summary>
        public override string GetSessionName(IHttpAqContext webctx)
        {
            // TODO: SessionOptions.CookieName
            // var config = ((Context)webctx).GetService<IConfigureOptions<Microsoft.AspNetCore.Builder.SessionOptions>>();
            
            return SessionDefaults.CookieName;
        }

        /// <summary>
        /// Sets the session name.
        /// </summary>
        public override bool SetSessionName(IHttpAqContext webctx, string name) => false; // throw new NotSupportedException();

        public override string HandlerName => "AspNetCore";

        /// <summary>
        /// Checks if sessions were configured.
        /// </summary>
        public override bool IsEnabled(IHttpAqContext webctx)
        {
            var httpctx = GeHttpContext(webctx);
            try
            {
                var session = httpctx.Session; // throws if session is not configured
                return session != null;
            }
            catch
            {
                return false;
            }
        }

        public override void Abandon(IHttpAqContext webctx)
        {
            // abandon asp.net core session
            var isession = GeHttpContext(webctx).Session;
            if (isession != null)
            {
                isession.Clear();
                isession.CommitAsync()
                    .GetAwaiter()
                    .GetResult();
            }
        }

        public override string GetSessionId(IHttpAqContext webctx)
        {
            var isession = GeHttpContext(webctx).Session;
            if (isession != null)
            {
                return isession.Id;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Initiates the session.
        /// </summary>
        public override bool StartSession(AqContext ctx, IHttpAqContext webctx)
        {
            if (base.StartSession(ctx, webctx))
            {
                //var httpctx = GeHttpContext(webctx);

                //if (httpctx.Session is SharedSession)
                //{
                //    // unexpected; session already bound
                //}
                //else
                //{
                //    // overwrite ISession
                //    httpctx.Session = new SharedSession(httpctx.Session, ctx.Session);
                //}

                //
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Close the session (either abandon or persist).
        /// </summary>
        public override void CloseSession(AqContext ctx, IHttpAqContext webctx, bool abandon)
        {
            base.CloseSession(ctx, webctx, abandon);

            // set the original ISession back
            var httpctx = GeHttpContext(webctx);
            // if (httpctx.Session is SharedSession shared && ReferenceEquals(shared.PhpSession, ctx.Session))
            // {
            //     httpctx.Session = shared.UnderlayingSession;
            // }
        }
    }
}
