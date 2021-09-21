using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Core.Contracts.Instance;

namespace Aquila.Core.Network
{
    /// <summary>
    /// Менеджер рабочих сред с возможностью разделения доступа к ним 
    /// </summary>
    public class FilteredInstanceManager : IPlatformInstanceManager
    {
        private IPlatformInstanceManager _manager;
        private Func<IInstance, bool> _filter;

        /// <summary>
        /// Создает менеджер с разделенным доступом к средам
        /// </summary>
        /// <param name="manager">Менеджер сред</param>
        /// <param name="filter">Фильтр</param>
        public FilteredInstanceManager(IPlatformInstanceManager manager,
            Func<IInstance, bool> filter)
        {
            _manager = manager;
            _filter = filter;
        }

        public void AddInstance(IStartupConfig config)
        {
            _manager.AddInstance(config);
        }

        public IPlatformInstance GetInstance(string name)
        {
            var env = _manager.GetInstance(name);
            if (_filter(env))
                return env;
            return null;
            throw new InvalidOperationException("Access denied.");
        }

        public IEnumerable<IPlatformInstance> GetInstances()
        {
            return _manager.GetInstances().Where(e => _filter(e));
        }
    }
}