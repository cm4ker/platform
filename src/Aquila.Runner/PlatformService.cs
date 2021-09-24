using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Aquila.Core.Network;
using Aquila.Core.Instance;
using Microsoft.Extensions.DependencyInjection;
using Aquila.Core.Contracts.Instance;
using Aquila.Core.Contracts.Network;
using Aquila.Core.Settings;
using Aquila.Logging;
using Microsoft.AspNetCore.Hosting;

namespace Aquila.Runner
{
    class PlatformService : IHostedService, IDisposable
    {
        private readonly ILogger<PlatformService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private IAccessPoint _accessPoint;

        public PlatformService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetRequiredService<ILogger<PlatformService>>();
        }

        public void Dispose()
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Info("Starting...");

            _accessPoint = _serviceProvider.GetRequiredService<IAccessPoint>();
            var envManager = _serviceProvider.GetRequiredService<IPlatformInstanceManager>();
            var webHost = _serviceProvider.GetRequiredService<IWebHost>();


            _accessPoint.Start();
            webHost.StartAsync(cancellationToken);

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