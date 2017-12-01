using System;
using System.Collections.Generic;
using ZenPlatform.Configuration.Data;

namespace ZenPlatform.Configuration
{
    /// <summary>
    /// Корень конфигурации
    /// </summary>
    public class PRootConfiguration
    {
        private List<PComponent> _dataSectionComponents;

        public PRootConfiguration()
        {

            _dataSectionComponents = new List<PComponent>();

            Id = Guid.NewGuid();
        }

        public PRootConfiguration(Guid id)
        {
            Id = id;
        }

        public string ConfigurationName { get; set; }

        public IEnumerable<PComponent> DataSectionComponents
        {
            get { return _dataSectionComponents; }
        }

        public void RegisterDataComponent(PComponent component)
        {
            _dataSectionComponents.Add(component);
        }

        public Guid Id { get; }
    }
}
