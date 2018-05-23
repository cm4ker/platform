using System.Collections.Generic;
using System.Xml.Serialization;

namespace ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration
{
    public class XmlConfSchedules
    {
        [XmlArray]
        [XmlArrayItem(Type = typeof(XmlConfFile))]
        public List<XmlConfFile> IncludedFiles { get; set; }
    }
}