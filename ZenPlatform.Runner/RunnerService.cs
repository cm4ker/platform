
using Microsoft.Extensions.Hosting;
using System;

using System.Threading;
using System.Threading.Tasks;
using ZenPlatform.Core.Network;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Logging;
using Microsoft.Extensions.DependencyInjection;
using ZenPlatform.Core.Settings;

namespace ZenPlatform.Runner
{
    class RunnerService : IHostedService, IDisposable
    {


        private readonly ILogger<RunnerService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private IAccessPoint _accessPoint;
        public RunnerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetRequiredService<ILogger<RunnerService>>();
        }


        public void Dispose()
        {

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Info("Starting...");

            _accessPoint = _serviceProvider.GetRequiredService<IAccessPoint>();

           var envManager = _serviceProvider.GetRequiredService<IPlatformEnvironmentManager>();

            //var route = new Route($"system\\test");
            //registrator.GetInvokeService(route.GetService()).Register(route, (c,a) => { return (int)a[0] + 1; });

            _accessPoint.Start();
            return Task.CompletedTask;
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Info("Stoping...");
            _accessPoint.Stop();

            _serviceProvider.GetRequiredService<ISettingsStorage>().Save();
            return Task.CompletedTask;
        }
    }
}
