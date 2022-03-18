// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Pipelines;
using System.IO.Pipes;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Aquila.LanguageServer.IntegrationTests.Helpers;
using Aquila.LanguageServer.IntegrationTests.Assertions;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nerdbank.Streams;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Utilities;
using DiagnosticSeverity = OmniSharp.Extensions.LanguageServer.Protocol.Models.DiagnosticSeverity;
using PipeOptions = System.IO.Pipes.PipeOptions;

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
                    //Arguments = exePath + " --wait-for-debugger",
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

        private static void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
        }

        private static void OnExit()
        {
        }

        private static void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
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


        public class TestLanguageClient : IDisposable
        {
            private ILanguageClient _client;
            private readonly MultipleMessageListener<PublishDiagnosticsParams> _publishDiagsListener;

            public TestLanguageClient(Stream inputStream, Stream outputStream, TestContext testContext)
            {
                _publishDiagsListener = new MultipleMessageListener<PublishDiagnosticsParams>();

                _client = LanguageClient.PreInit(options =>
                {
                    options
                        .WithRootPath(TestConstant.RootPath)
                        .WithInput(inputStream.UsePipeReader(pipeOptions: new System.IO.Pipelines.PipeOptions()))
                        .WithOutput(outputStream)
                        .OnInitialize((client, request, cancellationToken) =>
                        {
                            testContext.WriteLine("Language client initializing.");
                            return Task.CompletedTask;
                        })
                        .OnInitialized((client, request, response, cancellationToken) =>
                        {
                            testContext.WriteLine("Language client initialized.");
                            return Task.CompletedTask;
                        })
                        .OnStarted((client, cancellationToken) =>
                        {
                            testContext.WriteLine("Language client started.");
                            return Task.CompletedTask;
                        })
                        .OnLogTrace(
                            @params => testContext.WriteLine($"TRACE: {@params.Message} VERBOSE: {@params.Verbose}"))
                        .OnLogMessage(@params => testContext.WriteLine($"{@params.Type}: {@params.Message}"))
                        .OnPublishDiagnostics(_publishDiagsListener.AddMessage);

                    options.OnUnhandledException += exception => { testContext.WriteLine(exception.ToString()); };
                });
            }

            public ILanguageClient Client => _client;

            public async Task Init(CancellationToken cancellationToken)
            {
                await _client.Initialize(cancellationToken);
            }

            public Task<PublishDiagnosticsParams> WaitNextDiag()
            {
                return _publishDiagsListener.WaitNext();
            }

            public void Dispose()
            {
                _client?.Dispose();
            }
        }

        private async Task<TestLanguageClient> InitializeLanguageClient(Stream inputStream, Stream outputStream,
            CancellationToken cancellationToken)
        {
            var client = new TestLanguageClient(inputStream, outputStream, TestContext);
            await client.Init(cancellationToken);
            return client;
        }

        [TestMethod]
        public async Task ServerProcess_e2e_test_with_console_io()
        {
            var cancellationToken = GetCancellationTokenWithTimeout(TimeSpan.FromSeconds(60));
            using var process = StartServerProcessWithConsoleIO();

            try
            {
                var input = process.StandardOutput.BaseStream;
                var output = process.StandardInput.BaseStream;

                using var client = await InitializeLanguageClient(input, output, cancellationToken);
                await TestServer(client);
            }
            finally
            {
                //TestContext.WriteLine(process.StandardError.ReadToEnd());
                process.Kill(entireProcessTree: true);
                process.Dispose();
            }
        }


        public async Task TestServer(TestLanguageClient client)
        {
            var documentUri = DocumentUri.From(TestConstant.TestFilePath);
            var aqTextFile = @"
pub fn test() int
{
    return 0
}
";
            client.Client.DidOpenTextDocument(
                TextDocumentParamHelper.CreateDidOpenDocumentParams(documentUri, aqTextFile, 0));
            var publishDiagsResult = await client.WaitNextDiag();

            publishDiagsResult.Diagnostics.Should().SatisfyRespectively(
                d =>
                {
                    d.Range.Should().HaveRange((3, 12), (3, 12));
                    d.Should().HaveCodeAndSeverity("AQ4003", DiagnosticSeverity.Error);
                });
        }

        [TestMethod]
        public async Task ServerProcess_e2e_test_with_named_pipes_io()
        {
            var cancellationToken = GetCancellationTokenWithTimeout(TimeSpan.FromSeconds(60));

            var pipeName = Guid.NewGuid().ToString();
            using var pipeStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut,
                NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            using var process = StartServerProcessWithNamedPipeIo(pipeName, TestContext);
            try
            {
                await pipeStream.WaitForConnectionAsync(cancellationToken);
                using var client = await InitializeLanguageClient(pipeStream, pipeStream, cancellationToken);
                await TestServer(client);
            }
            finally
            {
                process.Kill(entireProcessTree: true);
                process.Dispose();
            }
        }
    }
}