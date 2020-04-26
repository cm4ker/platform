using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Core.Network
{
    public interface IChannelFactory
    {
        IChannel CreateChannel();
    }
}
