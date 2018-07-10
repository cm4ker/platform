using System.Xml.Serialization;

namespace ZenPlatform.Configuration.Structure
{
    /// <summary>
    /// Включение данных в компонент
    /// </summary>
    public class XCBlob
    {
        [XmlAttribute("Name")] public string Name { get; set; }
    }
}