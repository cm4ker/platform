using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Aquila.Core.Settings;
using Aquila.Logging;
using Microsoft.AspNetCore.Hosting;

namespace Aquila.Runner
{
    class AquilaHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<AquilaHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;
        
        public AquilaHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetRequiredService<ILogger<AquilaHostedService>>();
        }

        public void Dispose()
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Info("Starting...");

            var webHost = _serviceProvider.GetRequiredService<IWebHost>();
            
            webHost.StartAsync(cancellationToken);

            return Task.CompletedTask;
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Info("Stoping...");
            return Task.CompletedTask;
        }
    }
}