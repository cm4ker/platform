using System.Collections.Generic;
using ZenPlatform.Configuration.Data;

namespace ZenPlatform.Configuration
{
    /// <summary>
    /// Корень конфигурации
    /// </summary>
    public class PRootConfiguration
    {
        public PRootConfiguration()
        {
            DataSectionComponents = new List<PComponent>();
        }

        public string ConfigurationName { get; set; }

        public IList<PComponent> DataSectionComponents { get; set; }

        public void RegisterDataComponent(PComponent component)
        {
            DataSectionComponents.Add(component);
        }
    }
}
