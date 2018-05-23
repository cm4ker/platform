using System.Collections.Generic;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration;

namespace ZenPlatform.ReferenceComponent.Configuration.XmlConfiguration
{
    [XmlRoot("Reference")]
    public class XmlConfReference : XmlConfComponentBase
    {
        [XmlArray]
        [XmlArrayItem(ElementName = "Property", Type = typeof(XmlConfReferenceProperty))]
        public List<XmlConfReferenceProperty> Properties { get; set; }

    }

    public class XmlConfReferenceProperty : XmlConfComponentPropertyBase
    {
    }
}
