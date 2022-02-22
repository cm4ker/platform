using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aquila.CodeAnalysis;
using Aquila.LanguageServer.Utils;
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

            // if (args.Length > 0 && args[0] == "debug")
            // {
            // Debugger.Launch();
            // while (!Debugger.IsAttached)
            // {
            //     await Task.Delay(100).ConfigureAwait(false);
            // }
            // }

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Debug()
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
                .MinimumLevel.Verbose()
                .CreateLogger();

            Log.Logger.Information("Starting language server");

            IObserver<WorkDoneProgressReport> workDone = null!;


            var server = OSLanguageServer.PreInit(
                options =>
                    options
                        .WithInput(Console.OpenStandardInput())
                        .WithOutput(Console.OpenStandardOutput())
                        .ConfigureLogging(
                            x => x
                                .AddSerilog(Log.Logger)
                                .AddLanguageProtocolLogging()
                                .SetMinimumLevel(LogLevel.Debug)
                        )
                        .WithHandler<TextDocumentHandler>()
                        .WithHandler<DidChangeWatchedFilesHandler>()
                        .WithHandler<FoldingRangeHandler>()
                        .WithHandler<MyWorkspaceSymbolsHandler>()
                        .WithHandler<MyDocumentSymbolHandler>()
                        .WithHandler<SemanticTokensHandler>()
                        .WithServices(x => x.AddLogging(b => b.SetMinimumLevel(LogLevel.Trace)))
                        .WithServices(x =>
                        {
                            x.AddSingleton<IEventEmitter, LanguageServerEventEmitter>();
                            x.AddSingleton<IOmniSharpEnvironment, OmniSharpEnvironment>();
                            x.AddSingleton<DocumentVersions>();
                            x.AddSingleton<ProjectHolder>();
                            x.AddSingleton<CompilationManager>();
                        })
                        .OnInitialize((server, request, token) =>
                            {
                                server.GetService<ProjectHolder>()?
                                    .Initialize(PathUtils.NormalizePath(request.RootPath));
                                return Task.CompletedTask;
                            }
                        )
            );
            //var opt = new OptionsWrapper<DotNetCliOptions>(new DotNetCliOptions { LocationPaths = new[] { "" } });
            await server.Initialize(CancellationToken.None).ConfigureAwait(false);
            await server.WaitForExit.ConfigureAwait(false);
        }
    }
}