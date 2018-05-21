using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration
{
    [XmlRoot("Root")]
    public class XmlConfRoot
    {
        [XmlElement("ProjectId")]
        public Guid ProjectId { get; set; }


        [XmlElement("ProjectName")]
        public string ProjectName { get; set; }

        [XmlElement("ProjectVersion")]
        public string ProjectVersion { get; set; }

        [XmlElement(Type = typeof(XmlConfData), ElementName = "Data")]
        public XmlConfData Data { get; set; }

        [XmlElement]
        public XmlConfInterface Interface { get; set; }

        [XmlElement]
        public XmlConfRoles Roles { get; set; }

        [XmlElement]
        public XmlConfModules Modules { get; set; }

        [XmlElement]
        public XmlConfSchedules Schedules { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "Language", Type = typeof(XmlConfLanguage))]
        public List<XmlConfLanguage> Languages { get; set; }
    }
}
