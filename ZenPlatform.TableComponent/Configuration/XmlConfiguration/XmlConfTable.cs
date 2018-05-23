using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration;

namespace ZenPlatform.DocumentComponent.Configuration.XmlConfiguration
{
    [XmlRoot("Table")]
    public class XmlConfTable : XmlConfComponentBase
    {
      
        [XmlElement]
        public string NumericPattern { get; set; }

        [XmlElement]
        public bool CanBePosted { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "Property", Type = typeof(XmlConfTableProperty))]
        public List<XmlConfTableProperty> Properties { get; set; }

    }

    public class XmlConfTableProperty : XmlConfComponentPropertyBase
    {
    }
}
