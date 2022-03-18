using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Aquila.LanguageServer;

public class SemanticTokensHandler : SemanticTokensHandlerBase
{
    private readonly ILogger _logger;
    private readonly ProjectHolder _holder;
    private readonly SemanticTokensLegend legend = new();

    public SemanticTokensHandler(ILogger<SemanticTokensHandler> logger, ProjectHolder holder)
    {
        _logger = logger;
        _holder = holder;
    }

    protected override async Task Tokenize(SemanticTokensBuilder builder, ITextDocumentIdentifierParams identifier,
        CancellationToken cancellationToken)
    {
        using var typesEnumerator = RotateEnum(SemanticTokenType.Defaults).GetEnumerator();
        using var modifiersEnumerator = RotateEnum(SemanticTokenModifier.Defaults).GetEnumerator();
        // you would normally get this from a common source that is managed by current open editor, current active editor, etc.

        var handler = await _holder.GetHandlerAsync();
        var syntaxTree = handler.GetFile(DocumentUri.GetFileSystemPath(identifier));

        var content = (await syntaxTree.GetTextAsync(cancellationToken)).ToString();
        await Task.Yield();

        foreach (var (line, text) in content.Split('\n').Select((text, line) => (line, text)))
        {
            var parts = text.TrimEnd().Split(';', ' ', '.', '"', '(', ')');
            var index = 0;
            foreach (var part in parts)
            {
                typesEnumerator.MoveNext();
                modifiersEnumerator.MoveNext();
                if (string.IsNullOrWhiteSpace(part)) continue;
                index = text.IndexOf(part, index, StringComparison.Ordinal);
                builder.Push(line, index, part.Length, typesEnumerator.Current, modifiersEnumerator.Current);
            }
        }
    }

    protected override Task<SemanticTokensDocument> GetSemanticTokensDocument(ITextDocumentIdentifierParams @params,
        CancellationToken cancellationToken) =>
        Task.FromResult(new SemanticTokensDocument(this.legend));


    private IEnumerable<T> RotateEnum<T>(IEnumerable<T> values)
    {
        while (true)
        {
            foreach (var item in values)
                yield return item;
        }
    }


    protected override SemanticTokensRegistrationOptions CreateRegistrationOptions(SemanticTokensCapability capability,
        ClientCapabilities clientCapabilities) => new SemanticTokensRegistrationOptions
    {
        DocumentSelector = DocumentSelector.ForLanguage("aqlang"),
        Legend = this.legend,
        Full = new SemanticTokensCapabilityRequestFull
        {
            Delta = true
        },
        Range = true
    };
}

#pragma warning restore 618