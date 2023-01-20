using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Aquila.Core;
using Aquila.Core.Contracts.Network;
using Aquila.Core.Infrastructure.Settings;
using Aquila.Core.Network;
using Aquila.Logging;

namespace Aquila.Networking
{
    public class UserAccessPoint : IAccessPoint
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly AccessPointConfig _config;
        private readonly IList<INetworkListener> _listeners = new List<INetworkListener>();


        public UserAccessPoint(ILogger<UserAccessPoint> logger, IServiceProvider serviceProvider,
            ISettingsStorage settingsStorage)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _config = settingsStorage.Get<AppConfig>().AccessPoint;
        }

        public void Start()
        {
            foreach (var lisetnercfg in _config.Listener)
            {
                INetworkListener listener = lisetnercfg.Type switch
                {
                    PointType.Instance => (INetworkListener)_serviceProvider
                        .GetRequiredService<IDatabaseNetworkListener>(),
                    PointType.Admin => _serviceProvider.GetRequiredService<ITerminalNetworkListener>(),
                    PointType.Test => _serviceProvider.GetRequiredService<IDatabaseNetworkListener>(),
                    _ => throw new InvalidOperationException()
                };

                listener.Start(NetworkUtility.CreateIPEndPoint(lisetnercfg.Address));
                _listeners.Add(listener);
            }
        }

        public void Stop()
        {
            foreach (var listener in _listeners)
            {
                listener.Stop();
                listener.Dispose();
            }
        }
    }
}