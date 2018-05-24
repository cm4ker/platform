using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration
{

    [XmlRoot("Root")]
    public class XmlConfRoot : IXmlSerializable
    {
        public XmlConfRoot()
        {
            ProjectId = Guid.NewGuid();

            Data = new XmlConfData();
            Interface = new XmlConfInterface();
            Roles = new XmlConfRoles();
            Modules = new XmlConfModules();
            Schedules = new XmlConfSchedules();
            Languages = new List<XmlConfLanguage>();
        }

        [XmlElement("ProjectId")]
        public Guid ProjectId { get; set; }

        [XmlElement("ProjectName")]
        public string ProjectName { get; set; }

        [XmlElement("ProjectVersion")]
        public string ProjectVersion { get; set; }

        [XmlElement(Type = typeof(XmlConfData), ElementName = "Data")]
        public XmlConfData Data { get; set; }

        [XmlElement]
        public XmlConfInterface Interface { get; set; }

        [XmlElement]
        public XmlConfRoles Roles { get; set; }

        [XmlElement]
        public XmlConfModules Modules { get; set; }

        [XmlElement]
        public XmlConfSchedules Schedules { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "Language", Type = typeof(XmlConfLanguage))]
        public List<XmlConfLanguage> Languages { get; set; }

        /// <summary>
        /// Загрузить концигурацию
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static XmlConfRoot Load(string path)
        {
            using (var tr = new StreamReader(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(XmlConfRoot));
                return (XmlConfRoot)serializer.Deserialize(tr);
            }
        }

        /// <summary>
        /// Создать новую концигурацию
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static XmlConfRoot Create(string path)
        {
            return new XmlConfRoot()
            {
                ProjectId = Guid.NewGuid(),
                ProjectName = "Новый проект"
            };
        }

        /// <summary>
        /// Сохранить конфигурацию
        /// </summary>
        public void Save(string path)
        {
            using (var tr = new StreamWriter(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(XmlConfRoot));
                serializer.Serialize(tr, this);
            }
        }

        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {

        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
