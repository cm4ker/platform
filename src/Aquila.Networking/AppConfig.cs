using System;
using System.Collections.Generic;
using System.Text;
using Aquila.Core.Instance;
using Aquila.Core.Settings;

namespace Aquila.Core
{
    [ConfigFileName(Name = "Instance.config")]
    public class AppConfig
    {
        public AccessPointConfig AccessPoint { get; set; }

        public CacheServiceConfig CacheService { get; set; }
        public List<StartupConfig> Environments { get; set; }

        public AppConfig()
        {
            Environments = new List<StartupConfig>();
            AccessPoint = new AccessPointConfig();
        }
    }

    public class CacheServiceConfig
    {
        public string Address { get; set; }
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