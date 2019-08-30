using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Core.Assemlies
{
    [Serializable]
    public class AssemblyDescription
    {
        public int Id;
        public string AssemblyHash;
        public string ConfigurationHash;
        public string Name;
        public DateTime CreateDataTime;
    }
}
