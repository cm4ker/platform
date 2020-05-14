using System.Collections.Generic;
using System.Xml.Serialization;
using Aquila.Configuration.ConfigurationLoader.XmlConfiguration;
using Aquila.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types.Complex;

namespace Aquila.InformationRegisterComponent.Configuration.XmlConfiguration
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
