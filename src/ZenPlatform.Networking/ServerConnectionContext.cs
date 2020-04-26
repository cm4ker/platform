using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Core.Contracts;
using ZenPlatform.Core.Sessions;

namespace ZenPlatform.Core.Network
{
    public class ServerConnectionContext: IConnectionContext
    {
        public ISession Session { get; set; }
        public IConnection Connection { get; set; }
}
}
