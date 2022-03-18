using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Aquila.LanguageServer;

internal class ShutdownHandler : IShutdownHandler
{
    public Task<Unit> Handle(ShutdownParams request, CancellationToken cancellationToken)
    {
        //Process.GetCurrentProcess().Close();
        return Unit.Task;
    }
}