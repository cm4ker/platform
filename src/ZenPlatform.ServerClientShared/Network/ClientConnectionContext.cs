using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Core.Network
{
    public class ClientConnectionContext : IConnectionContext
    {
        public IConnection Connection { get; set; }
    }
}
