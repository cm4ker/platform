using System.Collections.Generic;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration;

namespace ZenPlatform.InformationRegisterComponent.Configuration.XmlConfiguration
{
    [XmlRoot("InformationRegister")]
    public class XmlConfInformationRegister : XmlConfComponentBase
    {
        [XmlArray]
        [XmlArrayItem(ElementName = "Property", Type = typeof(XmlConfInformationRegisterProperty))]
        public List<XmlConfInformationRegisterProperty> Properties { get; set; }

    }

    public class XmlConfInformationRegisterProperty : XmlConfComponentPropertyBase
    {
    }
}
