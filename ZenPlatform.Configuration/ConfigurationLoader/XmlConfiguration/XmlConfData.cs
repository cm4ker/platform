using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration
{
    [Serializable]
    public class XmlConfData : ISerializable
    {
        public XmlConfData()
        {
            IncludedFiles = new List<XmlConfFile>();
            Components = new List<XmlConfComponent>();
        }

        [XmlArray("IncludedFiles")]
        [XmlArrayItem(ElementName = "File", Type = typeof(XmlConfFile))]
        public List<XmlConfFile> IncludedFiles { get; set; }

        [XmlArray("Components")]
        [XmlArrayItem(ElementName = "Component", Type = typeof(XmlConfComponent))]
        public List<XmlConfComponent> Components { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}