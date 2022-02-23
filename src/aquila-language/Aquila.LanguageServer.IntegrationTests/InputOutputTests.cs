// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Aquila.LanguageServer.IntegrationTests.Helpers;
using Aquila.LanguageServer.IntegrationTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;

namespace Aquila.LanguageServer.IntegrationTests
{
    public static class TestConstant
    {
        public static string RootPath = Path.Combine(GetRootDirectory(), "Build", "library");

        public static string TestFilePath = Path.Combine(RootPath, "test.aq");


        static string GetRootDirectory()
        {
            var d = Directory.GetCurrentDirectory();
            while (!File.Exists(Path.Combine(d, "Aquila.sln")))
            {
                d = Path.GetDirectoryName(d);
            }

            return d;
        }
    }

    [TestClass]
    public class InputOutputTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private CancellationToken GetCancellationTokenWithTimeout(TimeSpan timeSpan)
            => CancellationTokenSource.CreateLinkedTokenSource(
                new CancellationTokenSource(timeSpan).Token,
                TestContext.CancellationTokenSource.Token).Token;

        private static Process StartServerProcessWithConsoleIO()
        {
            var exePath = typeof(LanguageServer.Program).Assembly.Location;

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = exePath,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                },
            };

            process.Start();

            return process;
        }

        private static Process StartServerProcessWithNamedPipeIo(string pipeName)
        {
            var exePath = typeof(LanguageServer.Program).Assembly.Location;

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"{exePath} --pipe {pipeName}",
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                },
            };

            process.Start();

            return process;
        }

        private static Process StartServerProcessWithSocketIo(int port)
        {
            var exePath = typeof(LanguageServer.Program).Assembly.Location;

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"{exePath} --socket {port}",
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                },
            };

            process.Start();

            return process;
        }

        private async Task<ILanguageClient> InitializeLanguageClient(Stream inputStream, Stream outputStream,
            MultipleMessageListener<PublishDiagnosticsParams> publishDiagnosticsListener,
            CancellationToken cancellationToken)
        {
            var client = LanguageClient.PreInit(options =>
            {
                options
                    .WithRootPath(TestConstant.RootPath)
                    .WithInput(inputStream)
                    .WithOutput(outputStream)
                    .OnInitialize((client, request, cancellationToken) =>
                    {
                        TestContext.WriteLine("Language client initializing.");

                        return Task.CompletedTask;
                    })
                    .OnInitialized((client, request, response, cancellationToken) =>
                    {
                        TestContext.WriteLine("Language client initialized.");
                        return Task.CompletedTask;
                    })
                    .OnStarted((client, cancellationToken) =>
                    {
                        TestContext.WriteLine("Language client started.");
                        return Task.CompletedTask;
                    })
                    .OnLogTrace(
                        @params => TestContext.WriteLine($"TRACE: {@params.Message} VERBOSE: {@params.Verbose}"))
                    .OnLogMessage(@params => TestContext.WriteLine($"{@params.Type}: {@params.Message}"))
                    .OnPublishDiagnostics(x => publishDiagnosticsListener.AddMessage(x));
            });

            await client.Initialize(cancellationToken);

            return client;
        }

        [TestMethod]
        public async Task ServerProcess_e2e_test_with_console_io()
        {
            var cancellationToken = GetCancellationTokenWithTimeout(TimeSpan.FromSeconds(60));
            var publishDiagsListener = new MultipleMessageListener<PublishDiagnosticsParams>();
            var documentUri = DocumentUri.From(TestConstant.TestFilePath);

            var aqTextFile = @"
public static int Test()
{
    return 0
}
";
            using var process = StartServerProcessWithConsoleIO();
            try
            {
                var input = process.StandardOutput.BaseStream;
                var output = process.StandardInput.BaseStream;

                using var client =
                    await InitializeLanguageClient(input, output, publishDiagsListener, cancellationToken);

                client.DidOpenTextDocument(
                    TextDocumentParamHelper.CreateDidOpenDocumentParams(documentUri, aqTextFile, 0));
                var publishDiagsResult = await publishDiagsListener.WaitNext(60000);

                publishDiagsResult.Diagnostics.Should().SatisfyRespectively(
                    d =>
                    {
                        d.Range.Should().HaveRange((3, 12), (3, 12));
                        d.Should().HaveCodeAndSeverity("AQ4003", DiagnosticSeverity.Error);
                    });
            }
            finally
            {
                process.Kill(entireProcessTree: true);
                process.Dispose();
            }
        }
    }
}