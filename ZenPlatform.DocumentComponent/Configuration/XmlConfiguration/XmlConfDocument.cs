using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration;

namespace ZenPlatform.DocumentComponent.Configuration.XmlConfiguration
{
    [XmlRoot("Document")]
    public class XmlConfDocument : XmlConfComponentBase
    {
        [XmlElement]
        public bool UseNumaration { get; set; }

        [XmlElement]
        public string NumericPattern { get; set; }

        [XmlElement]
        public bool CanBePosted { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "Property", Type = typeof(XmlConfDocumentProperty))]
        public List<XmlConfDocumentProperty> Properties { get; set; }

    }

    public class XmlConfDocumentProperty : XmlConfComponentPropertyBase
    {
    }

    [XmlRoot("DocumentRule")]
    public class XmlConfDocumentRule
    {
        [XmlElement]
        public bool AllowRead { get; set; }

        [XmlElement]
        public bool AllowView { get; set; }

        [XmlElement]
        public bool AllowAdd { get; set; }

        [XmlElement]
        public bool AllowDelete { get; set; }

        [XmlElement]
        public bool AllowInteractiveDelete { get; set; }

        [XmlElement]
        public bool AllowUpdate { get; set; }
    }
}
