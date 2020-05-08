using System;
using System.Collections.Generic;
using System.Text;
using Aquila.Core.Contracts;
using Aquila.Core.Sessions;

namespace Aquila.Core.Network
{
    public class ServerConnectionContext: IConnectionContext
    {
        public ISession Session { get; set; }
        public IConnection Connection { get; set; }
}
}
