using System.Xml.Serialization;
using ZenPlatform.Configuration.Contracts;

namespace ZenPlatform.Configuration.Structure
{

    public class XCLanguage : IXCLanguage
    {
        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public string Alias { get; set; }
    }
}