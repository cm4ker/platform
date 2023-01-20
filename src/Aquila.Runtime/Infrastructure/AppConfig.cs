using System.Collections.Generic;
using Aquila.Core.Infrastructure.Settings;

namespace Aquila.Core
{
    [ConfigFileName(Name = "Instance.json")]
    public class AppConfig
    {
        public AccessPointConfig AccessPoint { get; set; }
        public List<StartupConfig> Instances { get; set; }

        public AppConfig()
        {
            Instances = new List<StartupConfig>();
            AccessPoint = new AccessPointConfig();
        }
    }

    public enum PointType
    {
        Instance,
        Admin,
        Test
    }

    public class AccessPointConfig
    {
        public List<ListenerConfig> Listener { get; set; }

        public AccessPointConfig()
        {
            Listener = new List<ListenerConfig>();
        }
    }

    public class ListenerConfig
    {
        public string Address { get; set; }
        public PointType Type { get; set; }
    }
}