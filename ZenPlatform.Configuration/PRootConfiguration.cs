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
            DataSection = new List<PObjectType>();
        }

        public string ConfigurationName { get; set; }
        public IEnumerable<PObjectType> DataSection { get; set; }
    }
}
