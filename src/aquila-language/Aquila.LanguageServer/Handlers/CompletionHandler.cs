using System.Threading;
using System.Threading.Tasks;
using Aquila.CodeAnalysis;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Aquila.LanguageServer;

public class CompletionHandler : CompletionHandlerBase
{
    private readonly ILogger<SemanticTokensHandler> _logger;
    private readonly ProjectHolder _holder;

    public CompletionHandler(ILogger<SemanticTokensHandler> logger, ProjectHolder holder)
    {
        _logger = logger;
        _holder = holder;
    }

    protected override CompletionRegistrationOptions CreateRegistrationOptions(CompletionCapability capability,
        ClientCapabilities clientCapabilities)
    {
        return new CompletionRegistrationOptions
        {
            DocumentSelector = DocumentSelector.ForLanguage("aqlang")
        };
    }

    public override async Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken)
    {
        var handler = await _holder.GetHandlerAsync();
        var syntaxTree = handler.GetFile(DocumentUri.GetFileSystemPath(request.TextDocument));

        var linePos = request.Position;
        var pos = syntaxTree.GetOffset(new LinePosition(linePos.Line - 1, linePos.Character - 1));


        var syntax = syntaxTree.FindTokenOrEndToken(pos, CancellationToken.None);
        handler.Compilation.GetSemanticModel(syntaxTree);

        return new CompletionList(new[]
            { new CompletionItem { Label = "test", Detail = "Hello this is test completion" } });
    }

    public override Task<CompletionItem> Handle(CompletionItem request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new CompletionItem { Label = "test", Detail = "Hello this is test" });
    }
}