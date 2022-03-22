using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SymbolKind = OmniSharp.Extensions.LanguageServer.Protocol.Models.SymbolKind;
using Loca = OmniSharp.Extensions.LanguageServer.Protocol.Models.Location;

namespace Aquila.LanguageServer;

internal class SymbolExtractor : AquilaSyntaxWalker
{
    public List<SymbolInformationOrDocumentSymbol> Symbols { get; set; } = new();

    public override void VisitImportDecl(ImportDecl node)
    {
        Symbols.Add(new DocumentSymbol()
        {
            Kind = SymbolKind.Namespace,
            Detail = node.Name.ToString(),
            Range = node.Name.GetLocation().AsRange2(),
            SelectionRange = node.Name.GetLocation().AsRange2()
        });
    }
}

internal class DocumentSymbolHandler : IDocumentSymbolHandler
{
    private readonly ILogger<DocumentSymbolHandler> _logger;
    private readonly ProjectHolder _holder;

    public DocumentSymbolHandler(ILogger<DocumentSymbolHandler> logger, ProjectHolder holder)
    {
        _logger = logger;
        _holder = holder;
    }

    public async Task<SymbolInformationOrDocumentSymbolContainer> Handle(DocumentSymbolParams request,
        CancellationToken cancellationToken)
    {
        var resultList = new HashSet<ISymbol>();

        // you would normally get this from a common source that is managed by current open editor, current active editor, etc.
        var handler = await _holder.GetHandlerAsync();
        var tree = handler.GetFile(DocumentUri.GetFileSystemPath(request));
        var root = tree.GetCompilationUnitRoot();

        var model = handler.Compilation.GetSemanticModel(tree);

        foreach (var node in root.DescendantNodesAndSelf())
        {
            switch (node.Kind())
            {
                case SyntaxKind.IdentifierEx:
                    ISymbol symbol = model.GetSymbolInfo(node).Symbol;

                    if (symbol != null && resultList.Add(symbol))
                    {
                    }

                    break;
            }
        }

        SymbolExtractor c = new SymbolExtractor();
        c.Visit(tree.GetCompilationUnitRoot());

        // await Task.Delay(2000, cancellationToken);
        return c.Symbols;
    }

    public DocumentSymbolRegistrationOptions GetRegistrationOptions(DocumentSymbolCapability capability,
        ClientCapabilities clientCapabilities) => new DocumentSymbolRegistrationOptions
    {
        DocumentSelector = DocumentSelector.ForLanguage("aqlang")
    };
}