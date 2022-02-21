using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Aquila.LanguageServer
{
    internal class DidChangeWatchedFilesHandler : IDidChangeWatchedFilesHandler
    {
        private readonly ProjectHolder _holder;

        public DidChangeWatchedFilesHandler(ProjectHolder holder)
        {
            _holder = holder;
        }

        public DidChangeWatchedFilesRegistrationOptions GetRegistrationOptions() =>
            new DidChangeWatchedFilesRegistrationOptions();

        public Task<Unit> Handle(DidChangeWatchedFilesParams request, CancellationToken cancellationToken)
        {
            _holder.Invalidate();
            return Unit.Task;
        }

        public DidChangeWatchedFilesRegistrationOptions GetRegistrationOptions(
            DidChangeWatchedFilesCapability capability, ClientCapabilities clientCapabilities) =>
            new DidChangeWatchedFilesRegistrationOptions();
    }
}