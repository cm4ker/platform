using System.Collections.Generic;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types.Complex;

namespace ZenPlatform.AcumulateRegisterComponent.Configuration.XmlConfiguration
{
    [XmlRoot("AcсumulateRegister")]
    public class XmlConfAcumulateRegister : XCObjectTypeBase
    {
      
        [XmlElement]
        public string NumericPattern { get; set; }

        [XmlElement]
        public bool CanBePosted { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "Property", Type = typeof(XmlConfAcumulateRegisterProperty))]
        public List<XmlConfAcumulateRegisterProperty> Properties { get; set; }

    }

    public class XmlConfAcumulateRegisterProperty : XCObjectPropertyBase
    {
    }
}
