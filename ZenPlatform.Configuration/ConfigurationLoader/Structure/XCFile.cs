using System;
using System.Xml.Serialization;

namespace ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration
{
    public class XCFile
    {
        [XmlAttribute("Path")]
        public string Path { get; set; }

        [XmlAttribute("ComponentId")]
        public Guid ComponentId { get; set; }
    }
}