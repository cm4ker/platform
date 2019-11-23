using System.Collections.Generic;
using System.Xml.Serialization;

namespace ZenPlatform.Configuration.Structure
{
 

    /// <summary>
    /// Для UI
    /// </summary>
    public class XCInterface : IXCInterface
    {
        [XmlArray("IncludedFiles")]
        [XmlArrayItem(ElementName = "File", Type = typeof(IXCFile))]
        public List<IXCFile> IncludedFiles { get; set; }
    }
}