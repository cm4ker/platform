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
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Aquila.LanguageServer;

public class SemanticsTokensVisitor : AquilaSyntaxWalker
{
    private readonly SemanticTokensBuilder _builder;

    public SemanticsTokensVisitor(SemanticTokensBuilder builder)
    {
        _builder = builder;
    }

    public override void VisitImportDecl(ImportDecl node)
    {
        PushKeyword(node.ImportKeyword);
    }


    public override void VisitInvocationEx(InvocationEx node)
    {
        if (node.Expression is IdentifierEx ie)
            Push(ie.Identifier, SemanticTokenType.Function);

        VisitArgumentList(node.ArgumentList);
    }

    public override void VisitFuncDecl(FuncDecl node)
    {
        Push(node.FnKeyword, SemanticTokenType.Keyword);
        Push(node.Identifier, SemanticTokenType.Function);

        VisitParameterList(node.ParameterList);
        Visit(node.Body);
    }

    public override void VisitParameter(ParameterSyntax node)
    {
        Push(node.Identifier, SemanticTokenType.Variable);
        PushType(node.Type);
    }


    private void PushType(TypeEx type)
    {
        switch (type.Kind())
        {
            case SyntaxKind.IdentifierEx when type is IdentifierEx ie:
                Push(ie.Identifier, SemanticTokenType.Type);
                break;
            case SyntaxKind.PredefinedType when type is PredefinedTypeEx pt:
                Push(pt.Keyword, SemanticTokenType.Type);
                break;
        }
    }

    private void Push(SyntaxToken token, SemanticTokenType type, SemanticTokenModifier decl = default)
    {
        if (decl == default)
            decl = SemanticTokenModifier.Declaration;

        var range = token.GetLocation().AsRange2();
        _builder.Push(range, type, decl);
    }

    private void PushKeyword(SyntaxToken token)
    {
        Push(token, SemanticTokenType.Keyword);
    }
}

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
        var handler = await _holder.GetHandlerAsync();
        var syntaxTree = handler.GetFile(DocumentUri.GetFileSystemPath(identifier));

        new SemanticsTokensVisitor(builder).Visit(syntaxTree.GetCompilationUnitRoot());
    }

    protected override Task<SemanticTokensDocument> GetSemanticTokensDocument(ITextDocumentIdentifierParams @params,
        CancellationToken cancellationToken) =>
        Task.FromResult(new SemanticTokensDocument(this.legend));

    protected override SemanticTokensRegistrationOptions CreateRegistrationOptions(SemanticTokensCapability capability,
        ClientCapabilities clientCapabilities) => new SemanticTokensRegistrationOptions
    {
        DocumentSelector = DocumentSelector.ForLanguage("aqlang"),
        Legend = this.legend,
        Full = new SemanticTokensCapabilityRequestFull
        {
            Delta = false
        },
        Range = true
    };
}

#pragma warning restore 618