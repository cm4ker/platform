using System;
using System.Collections.Generic;
using System.Text;

namespace Aquila.Core.Network
{
    public interface IChannelFactory
    {
        IChannel CreateChannel();
    }
}
