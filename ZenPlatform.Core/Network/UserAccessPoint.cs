using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ZenPlatform.Core.Logging;
using ZenPlatform.Core.DI;
using Microsoft.Extensions.DependencyInjection;
using ZenPlatform.Core.Settings;

namespace ZenPlatform.Core.Network
{
    public class UserAccessPoint : IAccessPoint
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly AccessPointConfig _config;
        private readonly IList<ITCPListener> _listeners = new List<ITCPListener>();


        public UserAccessPoint(ILogger<UserAccessPoint> logger, IServiceProvider serviceProvider, ISettingsStorage settingsStorage)
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
                ITCPListener listener = _serviceProvider.GetRequiredService<ITCPListener>();
                TCPConnectionFactory connectionFactory = null;
                switch (lisetnercfg.Type)
                {
                    case ListenerType.User:
                        connectionFactory = _serviceProvider.GetRequiredService<UserTCPConnectionFactory>();
                        break;
                    case ListenerType.Admin:
                        connectionFactory = _serviceProvider.GetRequiredService<TCPConnectionFactory>();
                        break;
                    case ListenerType.Test:
                        connectionFactory = _serviceProvider.GetRequiredService<TCPConnectionFactory>();
                        break;
                    default:
                        throw new InvalidOperationException();

                }
                
                listener.Start(NetworkUtility.CreateIPEndPoint(lisetnercfg.Address), connectionFactory);
                _listeners.Add(listener);
            }
        }

        public void Stop()
        {
            foreach (var listener in _listeners)
            {
                listener.Stop();
            }
        }
    }
}