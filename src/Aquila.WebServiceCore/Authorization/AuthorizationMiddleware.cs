using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Aquila.WebServiceCore
{
    /// <summary>
    /// A middleware that enables authorization capabilities.
    /// </summary>
    public class AuthorizationMiddleware
    {
        // AppContext switch used to control whether HttpContext or endpoint is passed as a resource to AuthZ
        private const string SuppressUseHttpContextAsAuthorizationResource =
            "Microsoft.AspNetCore.Authorization.SuppressUseHttpContextAsAuthorizationResource";

        // Property key is used by Endpoint routing to determine if Authorization has run
        private const string AuthorizationMiddlewareInvokedWithEndpointKey =
            "__AuthorizationMiddlewareWithEndpointInvoked";

        private static readonly object AuthorizationMiddlewareWithEndpointInvokedValue = new object();

        private readonly RequestDelegate _next;
        private readonly IAuthorizationPolicyProvider _policyProvider;
        private readonly AquilaAuthorizationOptions _authorizationOptions;

        /// <summary>
        /// Initializes a new instance of <see cref="AuthorizationMiddleware"/>.
        /// </summary>
        /// <param name="next">The next middleware in the application middleware pipeline.</param>
        /// <param name="policyProvider">The <see cref="IAuthorizationPolicyProvider"/>.</param>
        public AuthorizationMiddleware(RequestDelegate next, IAuthorizationPolicyProvider policyProvider,
            IOptions<AquilaAuthorizationOptions> authOptions)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _policyProvider = policyProvider ?? throw new ArgumentNullException(nameof(policyProvider));
            _authorizationOptions = authOptions.Value;
        }

        /// <summary>
        /// Invokes the middleware performing authorization.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        public async Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // IMPORTANT: Changes to authorization logic should be mirrored in MVC's AuthorizeFilter
            var authorizeData = _authorizationOptions.GetData<IAuthorizeData>();
            var policy = await AuthorizationPolicy.CombineAsync(_policyProvider, authorizeData);
            if (policy == null)
            {
                await _next(context);
                return;
            }

            // Policy evaluator has transient lifetime so it fetched from request services instead of injecting in constructor
            var policyEvaluator = context.RequestServices.GetRequiredService<IPolicyEvaluator>();

            var authenticateResult = await policyEvaluator.AuthenticateAsync(policy, context);

            // Allow Anonymous skips all authorization
            if (_authorizationOptions.GetData<IAllowAnonymous>().Any())
            {
                await _next(context);
                return;
            }

            var authorizeResult =
                await policyEvaluator.AuthorizeAsync(policy, authenticateResult, context, resource: context);
            var authorizationMiddlewareResultHandler =
                context.RequestServices.GetRequiredService<IAuthorizationMiddlewareResultHandler>();
            await authorizationMiddlewareResultHandler.HandleAsync(_next, context, policy, authorizeResult);
        }
    }
}