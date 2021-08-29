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

        public void AddWorkInstance(IStartupConfig config)
        {
            _manager.AddWorkInstance(config);
        }

        public IInstance GetInstance(string name)
        {
            var env = _manager.GetInstance(name);
            if (_filter(env))
                return env;
            throw new InvalidOperationException("Access denied.");
        }

        public List<IInstance> GetInstanceList()
        {
            return _manager.GetInstanceList().Where(e => _filter(e)).ToList();
        }
    }
}