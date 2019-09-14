using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Core.Contracts;

namespace ZenPlatform.Core.Network
{
    public class RouteFactory : IRouteFactory
    {
        public Route Create(string path)
        {
            return new Route(path);
        }
    }
}
