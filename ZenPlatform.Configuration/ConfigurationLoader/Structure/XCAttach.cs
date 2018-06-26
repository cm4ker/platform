using System;
using System.Xml.Serialization;

namespace ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration
{
    public class XCAttach
    {
        [XmlAttribute]
        public XCRefMode Mode { get; set; }

        [XmlAttribute]
        public Guid DestenationId { get; set; }
    }
}