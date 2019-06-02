using System.Collections.Generic;
using System.Xml.Serialization;

namespace ZenPlatform.Configuration.Structure
{
    /// <summary>
    /// Для UI
    /// </summary>
    public class XCInterface
    {
        [XmlArray("IncludedFiles")]
        [XmlArrayItem(ElementName = "File", Type = typeof(XCFile))]
        public List<XCFile> IncludedFiles { get; set; }
    }
}