using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ZenPlatform.Core.Logging;
using ZenPlatform.ServerClientShared;
using ZenPlatform.ServerClientShared.DI;
using ZenPlatform.ServerClientShared.Logging;
using ZenPlatform.ServerClientShared.Network;
using Microsoft.Extensions.DependencyInjection;

namespace ZenPlatform.Core.Network
{
    public class UserAccessPoint : IAccessPoint
    {
        private readonly ILogger _logger;
        private bool _running;
        private Thread _thread;
        private readonly IServiceProvider _serviceProvider;
        private readonly AccessPointConfig _config;
        private readonly IList<IListener> _listeners = new List<IListener>();


        public UserAccessPoint(ILogger<UserAccessPoint> logger, IServiceProvider serviceProvider, IConfig<AccessPointConfig> config)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _config = config.Value;
        }

        public void Start()
        {

            foreach (var lisetnercfg in _config.Listeners)
            {
                IListener listener = null;
                switch (lisetnercfg.Type)
                {
                    case ListenerType.User:
                        listener = _serviceProvider.GetRequiredService<IUserListener>();
                        break;
                    case ListenerType.Admin:
                        listener = _serviceProvider.GetRequiredService<IUserListener>();
                        break;
                    default: 
                        listener = _serviceProvider.GetRequiredService<IUserListener>();
                        break;

                }
                
                listener.Start(NetworkUtility.CreateIPEndPoint(lisetnercfg.Address));
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
