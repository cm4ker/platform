using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Aquila.Core;
using Aquila.Core.Contracts.Network;
using Aquila.Core.Network;
using Aquila.Core.Settings;
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
            //_config.Listener.Add(new ListenerConfig() { Address = "127.0.0.1:12345", Type = ListenerType.Test });
            foreach (var lisetnercfg in _config.Listener)
            {
                INetworkListener listener = lisetnercfg.Type switch
                {
                    ListenerType.User => (INetworkListener) _serviceProvider
                        .GetRequiredService<IDatabaseNetworkListener>(),
                    ListenerType.Admin => _serviceProvider.GetRequiredService<ITerminalNetworkListener>(),
                    ListenerType.Test => _serviceProvider.GetRequiredService<IDatabaseNetworkListener>(),
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