using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Aquila.LanguageServer
{
    internal class FoldingRangeHandler : IFoldingRangeHandler
    {
        private readonly ILogger<FoldingRangeHandler> _logger;

        public FoldingRangeHandler(ILogger<FoldingRangeHandler> logger)
        {
            _logger = logger;
            _logger.LogInformation("Folding register");
        }

        public FoldingRangeRegistrationOptions GetRegistrationOptions() =>
            new FoldingRangeRegistrationOptions
            {
                DocumentSelector = DocumentSelector.ForLanguage("aqlang")
            };

        public Task<Container<FoldingRange>?> Handle(FoldingRangeRequestParam request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Create folding");

            return Task.FromResult<Container<FoldingRange>?>(
                new Container<FoldingRange>(
                    new FoldingRange
                    {
                        StartLine = 10,
                        EndLine = 20,
                        Kind = FoldingRangeKind.Region,
                        EndCharacter = 0,
                        StartCharacter = 0
                    }
                )
            );
        }

        public FoldingRangeRegistrationOptions GetRegistrationOptions(FoldingRangeCapability capability,
            ClientCapabilities clientCapabilities) =>
            new FoldingRangeRegistrationOptions
            {
                DocumentSelector = DocumentSelector.ForLanguage("aqlang")
            };
    }
}