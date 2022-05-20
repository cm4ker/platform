using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Aquila.LanguageServer;

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
            Contents = new MarkedStringsOrMarkupContent(new MarkedString("Hello hover!")),
            Range = new Range(request.Position, request.Position)
        };
    }
}