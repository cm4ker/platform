using System;
using System.Xml.Serialization;
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

        public XCBlob(string uri, string hash)
        {
            URI = new Uri(uri);
            Hash = hash;
        }

        public XCBlob(Uri uri, string hash)
        {
            URI = uri;
            Hash = hash;
        }

        [XmlAttribute("Name")] public string Name { get; set; }

        [XmlAttribute("URI")] public Uri URI { get; set; }

        [XmlAttribute("Hash")] public string Hash { get; set; }
    }
}