using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

#pragma warning disable CS0618

namespace Aquila.LanguageServer
{
    internal class TextDocumentHandler : TextDocumentSyncHandlerBase
    {
        private readonly ILogger<TextDocumentHandler> _logger;
        private readonly ILanguageServerConfiguration _configuration;
        private readonly ProjectHolder _holder;

        private readonly DocumentSelector _documentSelector = DocumentSelector.ForLanguage("aqlang");

        public TextDocumentHandler(ILogger<TextDocumentHandler> logger, ILanguageServerConfiguration configuration,
            ProjectHolder holder)
        {
            _logger = logger;
            _configuration = configuration;
            _holder = holder;

            _logger.LogInformation("Text document handler register");
        }

        public TextDocumentSyncKind Change { get; } = TextDocumentSyncKind.Full;

        public override async Task<Unit> Handle(DidChangeTextDocumentParams notification, CancellationToken token)
        {
            var handler = await _holder.GetHandlerAsync();

            string path = PathUtils.NormalizePath(notification.TextDocument.Uri.ToString());
            string text = notification.ContentChanges.First().Text;

            try
            {
                handler.UpdateFile(path, text);
            }
            catch (Exception ex)
            {
            }

            return Unit.Value;
        }

        public override async Task<Unit> Handle(DidOpenTextDocumentParams notification, CancellationToken token)
        {
            var handler = await _holder.GetHandlerAsync();

            string path = DocumentUri.GetFileSystemPath(notification.TextDocument);
            string text = notification.TextDocument.Text;

            try
            {
                handler.UpdateFile(path, text);
            }
            catch (Exception ex)
            {
            }

            return Unit.Value;
        }

        public override Task<Unit> Handle(DidCloseTextDocumentParams notification, CancellationToken token)
        {
            if (_configuration.TryGetScopedConfiguration(notification.TextDocument.Uri, out var disposable))
            {
                disposable.Dispose();
            }

            _logger.LogInformation("Hello from close document");
            return Unit.Task;
        }

        public override Task<Unit> Handle(DidSaveTextDocumentParams notification, CancellationToken token)
        {
            _logger.LogInformation("Hello from save document");
            return Unit.Task;
        }

        protected override TextDocumentSyncRegistrationOptions CreateRegistrationOptions(
            SynchronizationCapability capability, ClientCapabilities clientCapabilities) =>
            new TextDocumentSyncRegistrationOptions()
            {
                DocumentSelector = _documentSelector,
                Change = Change,
                Save = new SaveOptions() { IncludeText = true }
            };

        public override TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri) =>
            new TextDocumentAttributes(uri, "aqlang");
    }
}