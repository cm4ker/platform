using System.Collections.Generic;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types.Complex;

namespace ZenPlatform.InformationRegisterComponent.Configuration.XmlConfiguration
{
    [XmlRoot("InformationRegister")]
    public class XmlConfInformationRegister : XCObjectTypeBase
    {
        [XmlArray]
        [XmlArrayItem(ElementName = "Property", Type = typeof(XmlConfInformationRegisterProperty))]
        public List<XmlConfInformationRegisterProperty> Properties { get; set; }

    }

    public class XmlConfInformationRegisterProperty : XCObjectPropertyBase
    {
    }
}
