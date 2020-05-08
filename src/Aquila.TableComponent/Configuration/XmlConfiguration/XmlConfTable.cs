using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Aquila.Configuration.ConfigurationLoader.XmlConfiguration;
using Aquila.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types.Complex;

namespace Aquila.DocumentComponent.Configuration.XmlConfiguration
{
    [XmlRoot("Table")]
    public class XmlConfTable : XCObjectTypeBase
    {
      
        [XmlElement]
        public string NumericPattern { get; set; }

        [XmlElement]
        public bool CanBePosted { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "Property", Type = typeof(XmlConfTableProperty))]
        public List<XmlConfTableProperty> Properties { get; set; }

    }

    public class XmlConfTableProperty : XCObjectPropertyBase
    {
    }
}
