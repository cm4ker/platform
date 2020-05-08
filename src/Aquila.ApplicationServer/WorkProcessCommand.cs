//using Aquila.Core.Annotations;

using System;

namespace Aquila.WorkProcess
{
    public class WorkProcessCommand
    {

    }

    public class RouteCommand : WorkProcessCommand
    {
        public string RoutingKey { get; }
        public object RoutingParams { get; }

        public RouteCommand(string routingKey, object routingParams)
        {
            RoutingKey = routingKey;
            RoutingParams = routingParams;
        }



        
    }


}