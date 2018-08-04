using System.Xml.Serialization;

namespace ZenPlatform.Configuration.Structure
{
    /// <summary>
    /// Включение данных в компонент
    /// </summary>
    public class XCBlob
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