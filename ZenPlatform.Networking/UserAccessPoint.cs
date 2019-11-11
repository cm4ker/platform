using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using ZenPlatform.Core;
using ZenPlatform.Core.Logging;
using ZenPlatform.Core.Network;
using ZenPlatform.Core.Settings;

namespace ZenPlatform.Networking
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
                INetworkListener listener = _serviceProvider.GetRequiredService<INetworkListener>();

                var connectionFactory = lisetnercfg.Type switch
                {
                    ListenerType.User => _serviceProvider.GetRequiredService<UserConnectionFactory>(),
                    ListenerType.Admin => _serviceProvider.GetRequiredService<ServerConnectionFactory>(),
                    ListenerType.Test => _serviceProvider.GetRequiredService<ServerConnectionFactory>(),
                    _ => throw new InvalidOperationException()
                };

                listener.Start(NetworkUtility.CreateIPEndPoint(lisetnercfg.Address), connectionFactory);
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