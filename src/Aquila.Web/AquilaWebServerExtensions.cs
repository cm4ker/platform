using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Aquila.AspNetCore.Web;
using Aquila.Core.Authentication;
using Aquila.Core.CacheService;
using Aquila.Core.Instance;
using Aquila.Core.Migration;
using Aquila.Core.Settings;
using Aquila.Core.Utilities;
using Aquila.Data;
using Aquila.Logging;
using Aquila.Migrations;
using Aquila.Web.Swagger;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Swashbuckle.AspNetCore.Swagger;
using RouteData = Microsoft.AspNetCore.Routing.RouteData;

namespace Aquila.Web
{
    public static class AquilaWebServerExtensions
    {
        public static IApplicationBuilder UseAquila(this IApplicationBuilder builder, WebHostBuilderContext app)
        {
            builder.UseRouting();

            builder.UseEndpoints(o =>
            {
                o.MapControllers();
                o.MapDefaultControllerRoute();
                o.MapBlazorHub();
                //o.MapFallbackToPage("/_Host");
                o.MapGet("/hello", async context => { });
            });


            builder.UseStaticFiles();
            builder.UseAuthentication();

            if (app.HostingEnvironment.IsDevelopment())
            {
                builder.UseSwagger(o => { });
                builder.UseSwaggerUI(options =>
                {
                    //TODO: need use {instance} pattern
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "server");
                    options.RoutePrefix = "swagger";
                });
            }

            builder.MapMiddleware<AquilaHandlerMiddleware>("default", "metadata",
                "api/{instance}/metadata", options => options.AddAuthorizeData(policy: "UserRequired"));

            builder.MapMiddleware<AquilaHandlerMiddleware>("default", "migrate",
                "api/{instance}/migrate", options => options.AddAuthorizeData(policy: "UserRequired"));

            builder.MapMiddleware<AquilaHandlerMiddleware>("default", "user",
                "api/{instance}/user", options => options.AddAuthorizeData(policy: "UserRequired"));

            builder.MapMiddleware<AquilaHandlerMiddleware>("default", "endpoints",
                "api/{instance}/endpoints/{method}", options => options.AddAuthorizeData(policy: "UserRequired"));

            builder.MapMiddleware<AquilaHandlerMiddleware>("default", "deploy",
                "api/{instance}/deploy", options => options.AddAuthorizeData(policy: "UserRequired"));

            builder.MapMiddleware<AquilaHandlerMiddleware>("default", "view",
                "{instance}/{view}", options => options.AddAuthorizeData(policy: "UserRequired"));

            return builder.MapMiddleware<AquilaHandlerMiddleware>("default", "crud",
                "api/{instance}/{object}/{method}/{id?}", options => options.AddAuthorizeData(policy: "UserRequired"));
        }

        private static IApplicationBuilder MapMiddleware<T>(this IApplicationBuilder builder, string name,
            string areaName, string template)
        {
            return builder.MapMiddleware<T>(name, areaName, template, options => { });
        }

        private static IApplicationBuilder MapMiddleware<T>(this IApplicationBuilder builder, string name,
            string areaName, string template, Action<AquilaAuthorizationOptions> configureAuthorisation)
        {
            var routeBuilder = new RouteBuilder(builder);

            var defaultsDictionary = new RouteValueDictionary() { { "area", areaName } };
            var constraintsDictionary = new RouteValueDictionary() { { "area", areaName } };

            var nested = builder.New();

            var authOpt = new AquilaAuthorizationOptions();
            configureAuthorisation(authOpt);
            nested.UseAuthorization(authOpt);


            nested.UseMiddleware<T>();
            nested.UseRouting();
            nested.UseEndpoints(o => { });

            var route = new Route(
                new RouteHandler(nested.Build()),
                name,
                template,
                defaults: defaultsDictionary,
                constraints: constraintsDictionary,
                dataTokens: null,
                inlineConstraintResolver: routeBuilder.ServiceProvider
                    .GetRequiredService<IInlineConstraintResolver>());


            routeBuilder.Routes.Add(route);
            builder.UseRouter(routeBuilder.Build());

            return builder;
        }

        public static IApplicationBuilder UseAuthorization(this IApplicationBuilder app)
        {
            return app.UseAuthorization(new AquilaAuthorizationOptions());
        }

        public static IApplicationBuilder UseAuthorization(this IApplicationBuilder app,
            AquilaAuthorizationOptions authorizationOptions)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (authorizationOptions == null)
            {
                throw new ArgumentNullException(nameof(authorizationOptions));
            }

            return app.UseMiddleware<AuthorizationMiddleware>(Options.Create(authorizationOptions));
        }

        public static IServiceCollection AddAquila(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            //for working with files
            services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });

            services.AddScoped<DataContextManager>();
            services.AddScoped<AqUserManager>();

            services.AddSingleton<AqInstanceManager>();
            services.AddScoped<AqInstance>();
            services.AddScoped<AqMigrationManager>();
            services.AddScoped<AqAuthenticationManager>();

            services.AddSingleton<AqInstanceManager, AqInstanceManager>();
            services.AddSingleton<ISettingsStorage, FileSettingsStorage>();


            services.AddTransient(typeof(ILogger<>), typeof(SimpleConsoleLogger<>));

            services.AddSingleton<ICacheService, DictionaryCacheService>();

            // services.TryAddEnumerable(
            //     ServiceDescriptor.Transient<IApiDescriptionProvider, AquilaApplicationModelProvider>());

            services.AddSingleton<AquilaApiHolder>();
            services.AddSingleton<AquilaHandlerMiddleware>();


            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddRouting();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("UserRequired", policy => policy.RequireAuthenticatedUser());
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.LoginPath = "/login";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(50);
                    options.SlidingExpiration = false;
                })
                .AddOpenIdConnect(options =>
                {
                    //TODO: make it optional by the settings

                    // Note: these settings must match the application details
                    // inserted in the database at the server level.
                    options.ClientId = "mvc";
                    options.ClientSecret = "901564A5-E7FE-42CB-B10D-61EF6A8F3654";

                    options.RequireHttpsMetadata = false;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SaveTokens = true;

                    // Use the authorization code flow.
                    options.ResponseType = OpenIdConnectResponseType.Code;
                    options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;

                    // Note: setting the Authority allows the OIDC client middleware to automatically
                    // retrieve the identity provider's configuration and spare you from setting
                    // the different endpoints URIs or the token validation parameters explicitly.
                    options.Authority = "https://localhost:44313/";

                    options.Scope.Add("email");
                    options.Scope.Add("roles");

                    options.SecurityTokenValidator = new JwtSecurityTokenHandler
                    {
                        // Disable the built-in JWT claims mapping feature.
                        InboundClaimTypeMap = new Dictionary<string, string>()
                    };

                    options.TokenValidationParameters.NameClaimType = "name";
                    options.TokenValidationParameters.RoleClaimType = "role";
                });

            services.AddControllersWithViews();

            services.AddHttpClient();


            services.TryAddTransient<ISwaggerProvider, AquilaSwaggerGenerator>();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            ContextExtensions.CurrentContextProvider = HttpContextExtension.GetOrCreateContext;

            return services;
        }
    }
}