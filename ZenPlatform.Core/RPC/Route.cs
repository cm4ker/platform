using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Core.RPC
{
    public class Route
    {
        internal Route(string serviceName, string methodName)
        {
            ServiceName = serviceName;
            MethodName = methodName;
        }

        public string ServiceName { get; set; }
        public string MethodName { get; set; }

    }
}
