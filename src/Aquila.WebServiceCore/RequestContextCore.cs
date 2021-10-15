using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Aquila.AspNetCore.Web.Session;
using Aquila.Core;
using Aquila.Core.Instance;
using Aquila.Core.Utilities;

namespace Aquila.AspNetCore.Web
{
    /// <summary>
    /// Runtime context for ASP.NET Core request.
    /// </summary>
    sealed class AqHttpContext : AqContext, IHttpAqContext
    {
        readonly HttpContext _httpctx;

        public AqHttpContext(HttpContext httpcontext, AqInstance instance) : base(instance)
        {
            Debug.Assert(httpcontext != null);

            _httpctx = httpcontext;

            httpcontext.Items[HttpContextItemKey] = this;
            var bodyControl = httpcontext.Features.Get<IHttpBodyControlFeature>();
            if (bodyControl != null)
            {
                bodyControl.AllowSynchronousIO = true;
            }

            this.SetupHeaders();
        }


        #region Overrided

        /// <summary>Debug display string.</summary>
        protected override string DebugDisplay => $"{_httpctx.Request.Path.Value}{_httpctx.Request.QueryString.Value}";

        public override string User => _httpctx.User.Identity?.Name ?? base.User;
        public override IEnumerable<string> Roles => _httpctx.User.Claims.Select(x => x.Value);

        #endregion

        #region IHttpPhpContext

        /// <summary>Gets value indicating HTTP headers were already sent.</summary>
        bool IHttpAqContext.HeadersSent
        {
            get { return _httpctx.Response.HasStarted; }
        }

        void IHttpAqContext.SetHeader(string name, string value, bool append)
        {
            if (name.EqualsOrdinalIgnoreCase("content-length"))
            {
                // ignore content-length header, it is set correctly by middleware, using actual encoding
                return;
            }

            // specific cases:
            if (name.EqualsOrdinalIgnoreCase("location"))
            {
                _httpctx.Response.StatusCode = (int)System.Net.HttpStatusCode.Redirect; // 302
            }

            //
            var stringValue = new StringValues(value);

            if (append) // || name.EqualsOrdinalIgnoreCase("set-cookie")
            {
                // headers that can have multiple values:
                _httpctx.Response.Headers.Append(name, stringValue);
            }
            else
            {
                // replace semantic
                _httpctx.Response.Headers[name] = stringValue;
            }
        }

        void IHttpAqContext.RemoveHeader(string name)
        {
            _httpctx.Response.Headers.Remove(name);
        }

        void IHttpAqContext.RemoveHeaders()
        {
            _httpctx.Response.Headers.Clear();
        }

        /// <summary>Enumerates HTTP headers in current response.</summary>
        IEnumerable<KeyValuePair<string, string>> IHttpAqContext.GetHeaders()
        {
            return _httpctx.Response.Headers.Select(pair =>
                new KeyValuePair<string, string>(pair.Key, pair.Value.ToString()));
        }

        IEnumerable<KeyValuePair<string, IEnumerable<string>>> IHttpAqContext.RequestHeaders
        {
            get
            {
                return _httpctx.Request.Headers.Select(header =>
                    new KeyValuePair<string, IEnumerable<string>>(header.Key, header.Value));
            }
        }

        public string CacheControl
        {
            get => _httpctx.Response.Headers["cache-control"];
            set => _httpctx.Response.Headers["cache-control"] = new StringValues(value);
        }

        public event Action HeadersSending
        {
            add
            {
                if (_headersSending == null)
                {
                    _httpctx.Response.OnStarting(() =>
                    {
                        _headersSending?.Invoke();
                        return Task.CompletedTask;
                    });
                }

                _headersSending += value;
            }
            remove { _headersSending -= value; }
        }

        Action _headersSending;

        /// <summary>
        /// Gets or sets HTTP response status code.
        /// </summary>
        public int StatusCode
        {
            get { return _httpctx.Response.StatusCode; }
            set { _httpctx.Response.StatusCode = value; }
        }

        /// <summary>
        /// Stream with contents of the incoming HTTP entity body.
        /// </summary>
        Stream IHttpAqContext.InputStream => _httpctx.Request.Body;

        void IHttpAqContext.AddCookie(string name, string value, DateTimeOffset? expires, string path, string domain,
            bool secure, bool httpOnly, string samesite)
        {
            var cookie = new CookieOptions
            {
                Expires = expires,
                Path = path,
                Domain = string.IsNullOrEmpty(domain)
                    ? null
                    : domain, // IE, Edge: cookie with the empty domain was not passed to request
                Secure = secure,
                HttpOnly = httpOnly,
            };

            if (HttpContextHelpers.TryParseSameSite(samesite, out var samesitemode))
            {
                cookie.SameSite = samesitemode;
            }

            _httpctx.Response.Cookies.Append(name, value ?? string.Empty, cookie);
        }

        void IHttpAqContext.Flush(bool endRequest)
        {
            _httpctx.Response.Body.Flush();

            if (endRequest)
            {
                // // reset underlying output stream without disabling Output Buffering
                // InitOutput(null, enableOutputBuffering: IsOutputBuffered);
                //
                // // signal to continue request pipeline
                // RequestCompletionSource?.TrySetResult(RequestCompletionReason.ForceEnd);
            }
        }

        /// <summary>
        /// Gets max request size (upload size, post size) in bytes.
        /// Gets <c>0</c> if limit is not set.
        /// </summary>
        public long MaxRequestSize
        {
            get
            {
                var maxsize = _httpctx.Features.Get<IHttpMaxRequestBodySizeFeature>()?.MaxRequestBodySize;

                return maxsize ?? 30_000_000;
            }
        }

        /// <summary>
        /// Whether the underlaying connection is alive.
        /// </summary>
        public bool IsClientConnected => !_httpctx.RequestAborted.IsCancellationRequested;

        /// <summary>
        /// Gets or sets session handler for current context.
        /// </summary>
        AquilaSessionHandler IHttpAqContext.SessionHandler
        {
            get => _sessionhandler ?? AspNetCoreSessionHandler.Default;
            set
            {
                if (_sessionhandler != null && _sessionhandler != value)
                {
                    _sessionhandler.CloseSession(this, this, abandon: true);
                }

                _sessionhandler = value;
            }
        }

        AquilaSessionHandler _sessionhandler;

        /// <summary>
        /// Gets or sets session state.
        /// </summary>
        AquilaSessionState IHttpAqContext.SessionState { get; set; }

        #endregion

        #region Request Lifecycle

        /// <summary>
        /// Event signaling the request processing has been finished or cancelled.
        /// </summary>
        /// <remarks>
        /// End may occur when request finishes its processing or when event explicitly requested by user's code (See <see cref="IHttpAqContext.Flush(bool)"/>).
        /// </remarks>
        public TaskCompletionSource<RequestCompletionReason> RequestCompletionSource { get; } = new();

        /// <summary>
        /// Internal timer used to signalize the request has timeouted.
        /// </summary>
        private Timer _requestLimitTimer = null;

        /// <summary>
        /// Set the time limit of the request, from now. Any pending time limit will be cancelled.
        /// After the specified time span, <see cref="RequestCompletionSource"/> will be signaled with the state <see cref="Timeout"/>.
        /// </summary>
        /// <param name="span">
        /// Time span of the time limit.
        /// Use <see cref="Timeout.InfiniteTimeSpan"/> (or <c>-1</c> milliseconds) to cancel the pending time limit.
        /// </param>
        internal void TrySetTimeLimit(TimeSpan span)
        {
            if (_requestLimitTimer == null)
            {
                if (span != Timeout.InfiniteTimeSpan)
                {
                    _requestLimitTimer = new Timer(
                        state =>
                        {
                            var self = (AqHttpContext)state;
                            self.RequestCompletionSource.TrySetResult(RequestCompletionReason.Timeout);
                        },
                        this, span, Timeout.InfiniteTimeSpan);
                }
            }
            else
            {
                if (span != Timeout.InfiniteTimeSpan)
                {
                    _requestLimitTimer.Change(span, Timeout.InfiniteTimeSpan);
                }
                else
                {
                    _requestLimitTimer.Dispose();
                    _requestLimitTimer = null;
                }
            }
        }

        /// <inheritdoc/>
        public void ApplyExecutionTimeout(TimeSpan span) => TrySetTimeLimit(span);

        /// <summary>
        /// Disposes request resources.
        /// </summary>
        public override void Dispose()
        {
            if (_requestLimitTimer != null)
            {
                _requestLimitTimer.Dispose();
                _requestLimitTimer = null;
            }

            base.Dispose();
        }

        #endregion

        public IHttpAqContext HttpAqContext => this;

        /// <summary>
        /// Name of the server software as it appears in <c>$_SERVER[SERVER_SOFTWARE]</c> variable.
        /// </summary>
        public static string ServerSoftware => "ASP.NET Core Server";

        /// <summary>
        /// Informational string exposing technology powering the web request and version.
        /// </summary>
        static readonly string s_XPoweredBy = $"Aquila {ContextExtensions.GetRuntimeInformationalVersion()}";

        /// <summary>
        /// Unique key of item within <see cref="HttpContext.Items"/> associated with this <see cref="Context"/>.
        /// </summary>
        static object HttpContextItemKey => typeof(AqContext);

        /// <summary>
        /// Reference to current <see cref="HttpContext"/>.
        /// Cannot be <c>null</c>.
        /// </summary>
        public HttpContext HttpContext => _httpctx;


        /// <summary>
        /// Gets (non disposed) context associated to given <see cref="HttpContext"/>.
        /// </summary>
        internal static AqContext TryGetFromHttpContext(HttpContext httpctx)
        {
            if (httpctx != null && httpctx.Items.TryGetValue(HttpContextItemKey, out object obj) &&
                obj is AqContext ctx)
            {
                return ctx;
            }

            return null;
        }

        void SetupHeaders()
        {
            _httpctx.Response.Headers["X-Powered-By"] = new StringValues(s_XPoweredBy);
        }

        IHttpRequestFeature HttpRequestFeature => _httpctx.Features.Get<IHttpRequestFeature>();
        IHttpConnectionFeature HttpConnectionFeature => _httpctx.Features.Get<IHttpConnectionFeature>();
    }
}