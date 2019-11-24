using System.Collections.Generic;
using System.Xml.Serialization;

namespace ZenPlatform.Configuration.Structure
{
    public class XCSchedules : IXCSchedules
    {
        [XmlArray]
        [XmlArrayItem(Type = typeof(XCFile))]
        public List<IXCFile> IncludedFiles { get; set; }
    }
}