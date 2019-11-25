using System;
using System.Xml.Serialization;
using ZenPlatform.Configuration.Contracts;

namespace ZenPlatform.Configuration.Structure
{
   

    public class XCFile : IXCFile
    {
        [XmlAttribute("Path")] public string Path { get; set; }
    }
}