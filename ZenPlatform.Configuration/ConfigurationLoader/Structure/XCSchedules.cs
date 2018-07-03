using System.Collections.Generic;
using System.Xml.Serialization;

namespace ZenPlatform.Configuration.ConfigurationLoader.Structure
{
    public class XCSchedules
    {
        [XmlArray]
        [XmlArrayItem(Type = typeof(XCFile))]
        public List<XCFile> IncludedFiles { get; set; }
    }
}