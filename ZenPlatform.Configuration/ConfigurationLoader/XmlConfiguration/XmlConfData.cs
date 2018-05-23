using System.Collections.Generic;
using System.Xml.Serialization;

namespace ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration
{
    public class XmlConfData
    {
        [XmlArray("IncludedFiles")]
        [XmlArrayItem(ElementName = "File", Type = typeof(XmlConfFile))]
        public List<XmlConfFile> IncludedFiles { get; set; }

        [XmlArray("Components")]
        [XmlArrayItem(ElementName = "Component", Type = typeof(XmlConfComponent))]
        public List<XmlConfComponent> Components { get; set; }

    }
}