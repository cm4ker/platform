using System;
using System.Xml.Serialization;

namespace ZenPlatform.Configuration.Structure
{
    public class XCFile
    {
        [XmlAttribute("Path")]
        public string Path { get; set; }
    }
}