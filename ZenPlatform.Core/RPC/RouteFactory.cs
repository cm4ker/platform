using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ZenPlatform.Core.RPC
{
    public class RouteFactory
    {
        private RouteFactory() { }

        public static RouteFactory Instance { get; } = new RouteFactory();

        public Route GetRoute<T>(string methodName)
        {
            return new Route(typeof(T).FullName, methodName);
        }

        public Route GetRoute(MethodInfo method)
        {
            return new Route(method.DeclaringType.FullName, method.Name);
        }

        public Route GetRoute(string serviceName, string methodName)
        {
            return new Route(serviceName, methodName);
        }
    }
}
