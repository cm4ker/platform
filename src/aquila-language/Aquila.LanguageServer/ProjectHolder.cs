using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace Aquila.LanguageServer;

public class ProjectHolder
{
    private readonly ILanguageServerFacade _server;
    private ProjectHandler _handler;
    private string _rootPath;


    public ProjectHolder(ILanguageServerFacade server)
    {
        _server = server;
    }


    internal void Initialize(string rootPath)
    {
        _rootPath = rootPath;
    }

    internal async Task<ProjectHandler> GetHandlerAsync()
    {
        if (_handler == null)
        {
            Interlocked.CompareExchange(ref _handler, await GetHandlerCore(), null);
            _handler!.DocumentDiagnosticsChanged += DiagnosticsChanged;
        }

        return _handler;
    }

    private void DiagnosticsChanged(object? sender, ProjectHandler.DocumentDiagnosticsEventArgs e)
    {
        _server.TextDocument.PublishDiagnostics(new PublishDiagnosticsParams
        {
            Uri = PathUtils.NormalizePath(e.DocumentPath),
            Diagnostics = new Container<Diagnostic>(e.Diagnostics.ToDiagnostics())
        });
    }

    internal void Invalidate()
    {
        _handler?.Dispose();
        _handler = null;
    }

    private async Task<ProjectHandler> GetHandlerCore()
    {
        return await ProjectUtils.TryGetFirstAquilaProjectAsync(_rootPath, null);
    }
}