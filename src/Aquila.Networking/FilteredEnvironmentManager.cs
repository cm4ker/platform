using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Core.Contracts.Environment;
using Aquila.Core.Environment;

namespace Aquila.Core.Network
{
    
    /// <summary>
    /// Менеджер рабочих сред с возможностью разделения доступа к ним 
    /// </summary>
    public class FilteredEnvironmentManager : IPlatformEnvironmentManager
    {
        private IPlatformEnvironmentManager _manager;
        private Func<IEnvironment, bool> _filter;

        /// <summary>
        /// Создает менеджер с разделенным доступом к средам
        /// </summary>
        /// <param name="manager">Менеджер сред</param>
        /// <param name="filter">Фильтр</param>
        public FilteredEnvironmentManager(IPlatformEnvironmentManager manager,
            Func<IEnvironment, bool> filter)
        {
            _manager = manager;
            _filter = filter;
        }

        public void AddWorkEnvironment(IStartupConfig config)
        {
            _manager.AddWorkEnvironment(config);
        }

        public IEnvironment GetEnvironment(string name)
        {
            var env = _manager.GetEnvironment(name);
            if (_filter(env))
                return env;
            throw new InvalidOperationException("Access denied.");
        }

        public List<IEnvironment> GetEnvironmentList()
        {
            return _manager.GetEnvironmentList().Where(e => _filter(e)).ToList();
        }
    }
}