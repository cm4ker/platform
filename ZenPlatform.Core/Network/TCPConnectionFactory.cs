using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using ZenPlatform.Core.Logging;
using ZenPlatform.Core.Environment;
using System.Net.Sockets;

namespace ZenPlatform.Core.Network
{
    public class TCPConnectionFactory : ITCPConnectionFactory
    {
        protected readonly IServiceProvider _serviceProvider;

        public TCPConnectionFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected TCPServerConnection CreateConnection(TcpClient tcpClient, IEnvironmentManager environmentManager)
        {
            var connection = new TCPServerConnection(
                _serviceProvider.GetRequiredService<ILogger<TCPServerConnection>>(),
                _serviceProvider.GetRequiredService<IChannelFactory>(),
                tcpClient,
                environmentManager
                );

            return connection;
        }
        public virtual TCPServerConnection CreateConnection(TcpClient tcpClient)
        {
            var connection = CreateConnection(
                tcpClient,
                _serviceProvider.GetRequiredService<IEnvironmentManager>()
                );

            return connection;
        }
    }

    public class UserTCPConnectionFactory : TCPConnectionFactory
    {
        public UserTCPConnectionFactory(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override TCPServerConnection CreateConnection(TcpClient tcpClient)
        {
            return CreateConnection(tcpClient, new FilteredEnvironmentManager(_serviceProvider.GetRequiredService<IEnvironmentManager>(),
                env => { return env.GetType().Equals(typeof(WorkEnvironment)); }));
        }
    }



    public class FilteredEnvironmentManager : IEnvironmentManager
    {
        private IEnvironmentManager _manager;
        private Func<IEnvironment, bool> _filter;
        public FilteredEnvironmentManager(IEnvironmentManager manager, Func<IEnvironment, bool> filter)
        {
            _manager = manager;
            _filter = filter;
        }

        public void AddWorkEnvironment(StartupConfig config)
        {
            _manager.AddWorkEnvironment(config);
        }

        public IEnvironment GetEnvironment(string name)
        {
            var env = _manager.GetEnvironment(name);
            if (_filter(env))
                return env;
            throw new InvalidOperationException("Access denied.");
        }

        public List<IEnvironment> GetEnvironmentList()
        {
            return _manager.GetEnvironmentList().Where(e => _filter(e)).ToList();
        }

    }
}
