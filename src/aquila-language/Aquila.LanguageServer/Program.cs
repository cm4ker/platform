using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using Aquila.CodeAnalysis;
using Aquila.LanguageServer.Utils;
using Bicep.LanguageServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using OmniSharp;
using OmniSharp.Eventing;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server;
using OmniSharp.LanguageServerProtocol.Eventing;
using OmniSharp.LanguageServerProtocol.Handlers;
using OmniSharp.Options;
using OmniSharp.Services;
using Serilog;
using OSLanguageServer = OmniSharp.Extensions.LanguageServer.Server.LanguageServer;

namespace Aquila.LanguageServer
{
    internal class Program
    {
        private static async Task Main(string[] args) => await MainAsync(args);

        private static async Task MainAsync(string[] args)
        {
            await EnvironmentUtils.InitializeAsync();
            AssemblyLoadContext.Default.Resolving += Assembly_Resolving;

            var server = new Server(a => { });
            await server.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }


        private static Assembly Assembly_Resolving(AssemblyLoadContext ctx, AssemblyName name)
        {
            // I'm not proud with this but we need to get things working quickly
            // ... we should not reference those packages at all and make use of msbuild on user's machine

            // Sdk is attempting to load its versions from actual NuGetSdkResolver, but we have a different version,
            // and it does not load the ones located in 'C:\Program Files\dotnet\sdk' by itself ...

            if (name.Name == "NuGet.ProjectModel")
            {
                return typeof(NuGet.ProjectModel.BuildOptions).Assembly; // our version :/
            }

            if (name.Name == "NuGet.Frameworks")
            {
                return typeof(NuGet.Frameworks.NuGetFramework).Assembly; // our version :/
            }

            if (name.Name == "NuGet.Common")
            {
                return typeof(NuGet.Common.FileUtility).Assembly; // our version :/
            }

            if (name.Name == "NuGet.Packaging")
            {
                return typeof(NuGet.Packaging.FrameworkReference).Assembly; // our version :/
            }

            if (name.Name == "NuGet.Versioning")
            {
                return typeof(NuGet.Versioning.SemanticVersion).Assembly; // our version :/
            }

            return null;
        }
    }
}