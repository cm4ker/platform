using System.Xml;
using System.Xml.Serialization;

namespace ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration
{
    public class XmlConfLanguage
    {
        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public string Alias { get; set; }
    }
}