using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration
{

    [XmlRoot("Role")]
    public class XmlConfRole
    {
        [XmlElement]
        public Guid Id { get; set; }

        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public XmlConfPlatformRules PlatformRules { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "Rule")]
        public List<XmlConfRule> DataRules { get; set; }
    }


    public class XmlConfPlatformRules
    {
        [XmlElement]
        public bool IsDataAdministrator { get; set; }

        [XmlElement]
        public bool CanUpdateDatabase { get; set; }

        [XmlElement]
        public bool CanMonopolisticMode { get; set; }

        [XmlElement]
        public bool IsActiveUser { get; set; }

        [XmlElement]
        public bool CanActionLogView { get; set; }

        [XmlElement]
        public bool CanUseThinClient { get; set; }

        [XmlElement]
        public bool CanUseWebClient { get; set; }

        [XmlElement]
        public bool CanUseExternalConnection { get; set; }

        [XmlElement]
        public bool CanOpenExternalModules { get; set; }
    }

    public class XmlConfRule
    {
        [XmlAttribute]
        public Guid ObjectId { get; set; }

        [XmlElement]
        public string Content { get; set; }
    }
}
