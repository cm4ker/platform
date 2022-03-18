using System;
using System.Collections.Generic;
using System.Text;
using Aquila.Core.Contracts;

namespace Aquila.Core.Network
{
    public class RouteFactory : IRouteFactory
    {
        public Route Create(string path)
        {
            return new Route(path);
        }
    }
}
