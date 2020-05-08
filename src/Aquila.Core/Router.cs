using System;
using System.Collections.Generic;

namespace Aquila.Core
{
    /// <summary>
    /// Маршрутизатор комманд.
    /// В ядре отвечает за команду, которая будет выполнена в результате маршрутизации по ключу
    /// </summary>
    public class Router
    {
        private Dictionary<string, Delegate> _routed;
        private static Router _instance;

        private Router()
        {
        }

        public static Router Instance
        {
            get
            {
                _instance = _instance ?? new Router();
                return _instance;
            }
        }

        public void RegisterRoute(string routingKey, Delegate method)
        {
            _routed.Add(routingKey, method);
        }
    }
}