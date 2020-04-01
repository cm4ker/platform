using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Settings;

namespace ZenPlatform.Core
{
    [ConfigFileName(Name = "App.config")]
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

    public enum ListenerType
    {
        User,
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
        public ListenerType Type { get; set; }
    }
}