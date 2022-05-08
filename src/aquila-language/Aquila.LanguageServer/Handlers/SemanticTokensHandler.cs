using System.Threading;
using System.Threading.Tasks;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Location = Microsoft.CodeAnalysis.Location;

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
        if (node.Expression is MemberAccessEx ma)
        {
            Visit(ma.Expression);
            Push(ma.Name, SemanticTokenType.Function);
        }

        VisitArgumentList(node.ArgumentList);
    }


    public override void VisitMemberAccessEx(MemberAccessEx node)
    {
        Push(node.Name, SemanticTokenType.Property);
        Visit(node.Expression);
    }

    public override void VisitFuncDecl(FuncDecl node)
    {
        Push(node.FnKeyword, SemanticTokenType.Keyword);
        Push(node.Identifier, SemanticTokenType.Function);

        VisitParameterList(node.ParameterList);
        PushType(node.ReturnType);

        base.VisitFuncDecl(node);
    }

    public override void VisitIfStmt(IfStmt node)
    {
        PushKeyword(node.IfKeyword);
        if (node.Else != null)
            PushKeyword(node.Else.ElseKeyword);

        base.VisitIfStmt(node);
    }

    public override void VisitParameter(ParameterSyntax node)
    {
        Push(node.Identifier, SemanticTokenType.Parameter);
        PushType(node.Type);
    }

    private void PushType(TypeEx type)
    {
        if (type != null)
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

    public override void VisitAssignEx(AssignEx node)
    {
        Push(node.OperatorToken, SemanticTokenType.Operator);
        Visit(node.Left);
        Visit(node.Right);
    }

    private void Push(SyntaxToken token, SemanticTokenType type, SemanticTokenModifier decl = default)
    {
        if (decl == default)
            decl = SemanticTokenModifier.Declaration;

        var range = token.GetLocation().AsRange2();
        _builder.Push(range, type, decl);
    }

    private void Push(SyntaxNode node, SemanticTokenType type, SemanticTokenModifier decl = default)
    {
        if (decl == default)
            decl = SemanticTokenModifier.Declaration;

        var range = node.GetLocation().AsRange2();
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

public class HoverHandler : HoverHandlerBase
{
    protected override HoverRegistrationOptions CreateRegistrationOptions(HoverCapability capability,
        ClientCapabilities clientCapabilities)
    {
        return new HoverRegistrationOptions() { DocumentSelector = DocumentSelector.ForLanguage("aqlang") };
    }

    public override async Task<Hover> Handle(HoverParams request, CancellationToken cancellationToken)
    {
        return new Hover
        {
            Contents = new MarkedStringsOrMarkupContent(new MarkedString("Hello!!!")),
            Range = new Range(request.Position, request.Position)
        };
    }
}

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

        //TODO: need to get position of document form line and column

        return new CompletionList(new[]
            { new CompletionItem { Label = "test", Detail = "Hello this is test completion" } });
    }

    public override Task<CompletionItem> Handle(CompletionItem request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new CompletionItem { Label = "test", Detail = "Hello this is test" });
    }
}

#pragma warning restore 618