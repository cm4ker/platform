// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Aquila.LanguageServer.IntegrationTests.Helpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol;
using Aquila.LanguageServer.IntegrationTests.Assertions;

namespace Aquila.LanguageServer.IntegrationTests
{
    [TestClass]
    [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods",
        Justification = "Test methods do not need to follow this convention.")]
    public class DocumentSymbolTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task RequestDocumentSymbol_should_return_full_symbol_list()
        {
            var documentUri = DocumentUri.From(TestConstant.TestFilePath);
            var diagsReceived = new TaskCompletionSource<PublishDiagnosticsParams>();

            using var helper = await LanguageServerHelper.StartServerWithClientConnectionAsync(this.TestContext,
                options => { options.OnPublishDiagnostics(diags => { diagsReceived.SetResult(diags); }); });
            var client = helper.Client;

            // client opens the document
            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(documentUri, @"
import Entity;

pub fn test(a int) int
{
    return a;
}
", 0));

            // client requests symbols
            var symbols = await client.TextDocument.RequestDocumentSymbol(new DocumentSymbolParams
            {
                TextDocument = new TextDocumentIdentifier
                {
                    Uri = documentUri,
                },
            });

            symbols.Should().HaveCount(24);

            // client deletes the output and renames the resource
            client.TextDocument.DidChangeTextDocument(TextDocumentParamHelper.CreateDidChangeTextDocumentParams(
                documentUri, @"
import Entity;

pub fn test() int
{
    return 0;
}
", 1));

            // client requests symbols
            symbols = await client.TextDocument.RequestDocumentSymbol(new DocumentSymbolParams
            {
                TextDocument = new TextDocumentIdentifier
                {
                    Uri = documentUri,
                },
            });

            symbols.Should().HaveCount(24);
        }

    }
}