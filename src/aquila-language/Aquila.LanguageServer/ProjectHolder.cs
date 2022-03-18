using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace Aquila.LanguageServer;

public class ProjectHolder
{
    private readonly ILanguageServerFacade _serverFacade;
    private readonly  ILogger<ProjectHolder> _logger;
    private ProjectHandler _handler;
    private string _rootPath;


    public ProjectHolder(ILanguageServerFacade serverFacade, ILogger<ProjectHolder> logger)
    {
        _serverFacade = serverFacade;
        _logger = logger;
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
        _serverFacade.TextDocument.PublishDiagnostics(new PublishDiagnosticsParams
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
        return await ProjectUtils.TryGetFirstAquilaProjectAsync(_rootPath, _logger);
    }
}