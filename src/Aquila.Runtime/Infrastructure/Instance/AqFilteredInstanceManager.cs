using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Core.Instance;

namespace Aquila.Core.Network
{
    /// <summary>
    /// Менеджер рабочих сред с возможностью разделения доступа к ним 
    /// </summary>
    public class AqFilteredInstanceManager : IAqInstanceManager
    {
        private AqInstanceManager _manager;
        private Func<AqInstance, bool> _filter;

        /// <summary>
        /// Создает менеджер с разделенным доступом к средам
        /// </summary>
        /// <param name="manager">Менеджер сред</param>
        /// <param name="filter">Фильтр</param>
        public AqFilteredInstanceManager(AqInstanceManager manager,
            Func<AqInstance, bool> filter)
        {
            _manager = manager;
            _filter = filter;
        }

        public void AddInstance(StartupConfig config)
        {
            _manager.AddInstance(config);
        }

        public AqInstance GetInstance(string name)
        {
            var env = _manager.GetInstance(name);
            if (_filter(env))
                return env;
            return null;
            throw new InvalidOperationException("Access denied.");
        }

        public IEnumerable<AqInstance> GetInstances()
        {
            return _manager.GetInstances().Where(e => _filter(e));
        }
    }
}