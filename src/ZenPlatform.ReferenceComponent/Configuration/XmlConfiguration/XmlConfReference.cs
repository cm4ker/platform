using System.Collections.Generic;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types.Complex;

namespace ZenPlatform.ReferenceComponent.Configuration.XmlConfiguration
{
    [XmlRoot("Reference")]
    public class XmlConfReference : XCObjectTypeBase
    {
        [XmlArray]
        [XmlArrayItem(ElementName = "Property", Type = typeof(XmlConfReferenceProperty))]
        public List<XmlConfReferenceProperty> Properties { get; set; }

    }

    public class XmlConfReferenceProperty : XCObjectPropertyBase
    {
    }
}
