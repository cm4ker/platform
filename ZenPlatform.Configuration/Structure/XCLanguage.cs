﻿using System.Xml.Serialization;

namespace ZenPlatform.Configuration.Structure
{
    public class XCLanguage
    {
        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public string Alias { get; set; }
    }
}