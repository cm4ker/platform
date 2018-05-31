﻿using System.Collections.Generic;
using System.Xml.Serialization;

namespace ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration
{
    public class XmlConfInterface
    {
        [XmlArray("IncludedFiles")]
        [XmlArrayItem(ElementName = "File", Type = typeof(XmlConfFile))]
        public List<XmlConfFile> IncludedFiles { get; set; }
    }
}