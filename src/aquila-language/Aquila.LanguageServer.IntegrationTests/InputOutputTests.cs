// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Pipes;
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

        private static Process StartServerProcessWithNamedPipeIo(string pipeName, TestContext context)
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
            process.OutputDataReceived += (sender, args) => context.WriteLine(args.Data);


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

        [TestMethod]
        public async Task ServerProcess_e2e_test_with_named_pipes_io()
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
            var pipeName = Guid.NewGuid().ToString();
            using var pipeStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut,
                NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            using var process = StartServerProcessWithNamedPipeIo(pipeName, TestContext);
            try
            {
                await pipeStream.WaitForConnectionAsync(cancellationToken);

                using var client =
                    await InitializeLanguageClient(pipeStream, pipeStream, publishDiagsListener, cancellationToken);

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