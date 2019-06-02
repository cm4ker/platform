
using Microsoft.Extensions.Hosting;
using System;

using System.Threading;
using System.Threading.Tasks;
using ZenPlatform.Core.Network;
using ZenPlatform.ServerClientShared.DI;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Logging;
using ZenPlatform.ServerClientShared.Logging;

namespace ZenPlatform.Runner
{
    class RunnerService : IHostedService, IDisposable
    {


        private readonly ILogger<RunnerService> _logger;
        private readonly IDependencyResolver _dependencyResolver;
        private IAccessPoint _accessPoint;
        public RunnerService(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
            _logger = _dependencyResolver.Resolve<ILogger<RunnerService>>();
        }


        public void Dispose()
        {

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Info("Starting...");
            ///var registrator = _dependencyResolver.Resolve<IInvokeServiceManager>();
            _accessPoint = _dependencyResolver.Resolve<IAccessPoint>();

           var envManager = _dependencyResolver.Resolve<IEnvironmentManager>();

            //var route = new Route($"system\\test");
            //registrator.GetInvokeService(route.GetService()).Register(route, (c,a) => { return (int)a[0] + 1; });

            _accessPoint.Start();
            return Task.CompletedTask;
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Info("Stoping...");
            _accessPoint.Stop();
            return Task.CompletedTask;
        }
    }
}
