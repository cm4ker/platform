using System;
using System.Collections.Generic;
using System.Text;

namespace Aquila.Core.Network
{
    public class ClientConnectionContext : IConnectionContext
    {
        public IConnection Connection { get; set; }
    }
}
