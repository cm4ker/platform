using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration
{
    public class XmlConfComponent
    {
        [XmlAttribute]
        public Guid Id { get; set; }

        [XmlIgnore]
        public string Name { get; set; }

        [XmlIgnore]
        public XmlConfRoot Root { get; set; }

        [XmlElement]
        public XmlConfFile File { get; set; }

        [XmlArray("Attaches")]
        [XmlArrayItem(ElementName = "Attach", Type = typeof(XmlConfAttach))]
        public List<XmlConfAttach> Attaches { get; set; }
    }
}