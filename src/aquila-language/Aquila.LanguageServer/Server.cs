using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp;
using OmniSharp.Eventing;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Server;
using OmniSharp.LanguageServerProtocol.Eventing;
using OmniSharp.LanguageServerProtocol.Handlers;
using OmniSharp.Services;
using Serilog;
using OmnisharpLanguageServer = OmniSharp.Extensions.LanguageServer.Server.LanguageServer;

namespace Aquila.LanguageServer;

public class Server : IDisposable
{
    private readonly OmnisharpLanguageServer server;

    public Server(Action<LanguageServerOptions> onOptionsFunc)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Debug()
            .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
            .MinimumLevel.Verbose()
            .CreateLogger();

        Log.Logger.Information("Starting language server");

        server = OmnisharpLanguageServer.PreInit(options =>
        {
            options
                .ConfigureLogging(
                    x => x
                        .AddSerilog(Log.Logger)
                        .AddLanguageProtocolLogging()
                        .SetMinimumLevel(LogLevel.Debug)
                )
                .WithHandler<TextDocumentHandler>()
                .WithHandler<DidChangeWatchedFilesHandler>()
                .WithHandler<FoldingRangeHandler>()
                .WithHandler<WorkspaceSymbolsHandler>()
                .WithHandler<DocumentSymbolHandler>()
                .WithHandler<SemanticTokensHandler>()
                .WithServices(services => RegisterServices(services))
                .OnInitialize((lgsrv, request, token) =>
                    {
                        lgsrv.GetService<ProjectHolder>()?
                            .Initialize(PathUtils.NormalizePath(request.RootPath));
                        return Task.CompletedTask;
                    }
                );

            onOptionsFunc(options);
        });
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        await server.Initialize(cancellationToken).ConfigureAwait(false);

        server.LogInfo($"Running on processId {Environment.ProcessId}");

        await server.WaitForExit.ConfigureAwait(false);
    }

    private static void RegisterServices(IServiceCollection x)
    {
        // using type based registration so dependencies can be injected automatically
        // without manually constructing up the graph
        //x.AddLogging(b => { b.SetMinimumLevel(LogLevel.Trace); });
        x.AddSingleton<IEventEmitter, LanguageServerEventEmitter>();
        x.AddSingleton<IOmniSharpEnvironment, OmniSharpEnvironment>();
        x.AddSingleton<DocumentVersions>();
        x.AddSingleton<ProjectHolder>();
        x.AddSingleton<CompilationManager>();
    }

    public void Dispose()
    {
        server.Dispose();
    }
}