using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Core
{
    public class AppConfig
    {
        public AccessPointConfig AccessPoint { get; set; }
        public List<StartupConfig> Environments { get; set; }
    }

    public class AccessPointConfig
    {
        public string Address { get; set; }
    }
    

    
}
