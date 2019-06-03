using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.ServerClientShared.Network
{
    public class RouteFactory : IRouteFactory
    {
        public Route Create(string path)
        {
            return new Route(path);
        }
    }
}
