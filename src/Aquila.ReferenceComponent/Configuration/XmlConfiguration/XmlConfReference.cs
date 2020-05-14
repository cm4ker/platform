using System.Collections.Generic;
using System.Xml.Serialization;
using Aquila.Configuration.ConfigurationLoader.XmlConfiguration;
using Aquila.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types.Complex;

namespace Aquila.ReferenceComponent.Configuration.XmlConfiguration
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
