using System.Xml.Serialization;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure.Data;

namespace ZenPlatform.Configuration.Structure
{


    /// <summary>
    /// Включение данных в компонент
    /// </summary>
    public class XCBlob : IXCBlob
    {
        public XCBlob()
        {
        }

        public XCBlob(string name)
        {
            Name = name;
        }

        [XmlAttribute("Name")] public string Name { get; set; }
    }
}