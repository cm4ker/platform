//using ZenPlatform.Core.Annotations;

namespace ZenPlatform.WorkProcess
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