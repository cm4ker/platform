using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace ZenPlatform.Core.Network
{
    public class ChannelFactory: IChannelFactory
    {
        private IServiceProvider _serviceProvider;
        public ChannelFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IChannel CreateChannel()
        {
            return _serviceProvider.GetRequiredService<IChannel>();
        }
    }

    
}
