using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server;
using Serilog;

namespace Aquila.LanguageServer
{
    internal class Program
    {
        private static async Task Main(string[] args) => await MainAsync(args);

        private static async Task MainAsync(string[] args)
        {
            if (args.Length > 0 && args[0] == "debug")
            {
                Debugger.Launch();
                while (!Debugger.IsAttached)
                {
                    await Task.Delay(100).ConfigureAwait(false);
                }
            }

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Debug()
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
                .MinimumLevel.Verbose()
                .CreateLogger();

            Log.Logger.Information("Starting language server");

            IObserver<WorkDoneProgressReport> workDone = null!;

            var server = await OmniSharp.Extensions.LanguageServer.Server.LanguageServer.From(
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
                        .WithServices(
                            services =>
                            {
                                services.AddSingleton(
                                    new ConfigurationItem
                                    {
                                        Section = "typescript",
                                    }
                                ).AddSingleton(
                                    new ConfigurationItem
                                    {
                                        Section = "terminal",
                                    }
                                );
                            }
                        )
                        .OnInitialize(
                            async (server, request, token) =>
                            {
                                var manager = server.WorkDoneManager.For(
                                    request, new WorkDoneProgressBegin
                                    {
                                        Title = "Server is starting...",
                                        Percentage = 10,
                                    }
                                );
                                workDone = manager;

                                await Task.Delay(2000).ConfigureAwait(false);

                                manager.OnNext(
                                    new WorkDoneProgressReport
                                    {
                                        Percentage = 20,
                                        Message = "loading in progress"
                                    }
                                );
                            }
                        )
                        .OnInitialized(
                            async (server, request, response, token) =>
                            {
                                workDone.OnNext(
                                    new WorkDoneProgressReport
                                    {
                                        Percentage = 40,
                                        Message = "loading almost done",
                                    }
                                );

                                await Task.Delay(2000).ConfigureAwait(false);

                                workDone.OnNext(
                                    new WorkDoneProgressReport
                                    {
                                        Message = "loading done",
                                        Percentage = 100,
                                    }
                                );
                                workDone.OnCompleted();
                            }
                        )
                        .OnStarted(
                            async (languageServer, token) =>
                            {
                                using var manager = await languageServer.WorkDoneManager
                                    .Create(new WorkDoneProgressBegin { Title = "Aquila Language Server" })
                                    .ConfigureAwait(false);

                                manager.OnNext(new WorkDoneProgressReport
                                    { Message = "Hello from aquila language server" });
                                await Task.Delay(2000).ConfigureAwait(false);


                                var configuration = await languageServer.Configuration.GetConfiguration(
                                    new ConfigurationItem { Section = "typescript" },
                                    new ConfigurationItem { Section = "terminal" }
                                ).ConfigureAwait(false);

                                var baseConfig = new JObject();
                                foreach (var config in languageServer.Configuration.AsEnumerable())
                                {
                                    baseConfig.Add(config.Key, config.Value);
                                }

                                var scopedConfig = new JObject();
                                foreach (var config in configuration.AsEnumerable())
                                {
                                    scopedConfig.Add(config.Key, config.Value);
                                }
                            }
                        )
            ).ConfigureAwait(false);

            await server.WaitForExit.ConfigureAwait(false);
        }
    }
}