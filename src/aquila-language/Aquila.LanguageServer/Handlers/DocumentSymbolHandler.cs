using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Aquila.LanguageServer;

internal class DocumentSymbolHandler : IDocumentSymbolHandler
{
    private readonly ILogger<DocumentSymbolHandler> _logger;
    private readonly ProjectHolder _holder;

    public DocumentSymbolHandler(ILogger<DocumentSymbolHandler> logger, ProjectHolder holder)
    {
        _logger = logger;
        _holder = holder;
        _logger.LogInformation("Init!!!! MyDocumentSymbolHandler");
    }

    public async Task<SymbolInformationOrDocumentSymbolContainer> Handle(DocumentSymbolParams request,
        CancellationToken cancellationToken)
    {
        // you would normally get this from a common source that is managed by current open editor, current active editor, etc.
        var handler = await _holder.GetHandlerAsync();
        var content = handler.GetFile(DocumentUri.GetFileSystemPath(request));
        
        var lines = (await content.GetTextAsync(cancellationToken)).ToString().Split('\n');

        var symbols = new List<SymbolInformationOrDocumentSymbol>();
        for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            var line = lines[lineIndex];
            var parts = line.Split(' ', '.', '(', ')', '{', '}', '[', ']', ';');
            var currentCharacter = 0;
            foreach (var part in parts)
            {
                if (string.IsNullOrWhiteSpace(part))
                {
                    currentCharacter += part.Length + 1;
                    continue;
                }

                symbols.Add(
                    new DocumentSymbol
                    {
                        Detail = part,
                        Deprecated = true,
                        Kind = SymbolKind.Field,
                        Tags = new[] { SymbolTag.Deprecated },
                        Range = new Range(
                            new Position(lineIndex, currentCharacter),
                            new Position(lineIndex, currentCharacter + part.Length)
                        ),
                        SelectionRange =
                            new Range(
                                new Position(lineIndex, currentCharacter),
                                new Position(lineIndex, currentCharacter + part.Length)
                            ),
                        Name = part
                    }
                );
                currentCharacter += part.Length + 1;
            }
        }

        // await Task.Delay(2000, cancellationToken);
        return symbols;
    }

    public DocumentSymbolRegistrationOptions GetRegistrationOptions(DocumentSymbolCapability capability,
        ClientCapabilities clientCapabilities) => new DocumentSymbolRegistrationOptions
    {
        DocumentSelector = DocumentSelector.ForLanguage("aqlang")
    };
}