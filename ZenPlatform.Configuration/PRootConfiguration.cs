using SqlPlusDbSync.Configuration;

namespace ZenPlatform.Configuration
{
    /// <summary>
    /// Корень конфигурации
    /// </summary>
    public class PRootConfiguration
    {
        public string ConfigurationName { get; set; }
        public IEnumerable<PObjectType> DataSection { get; set; }
    }
}
