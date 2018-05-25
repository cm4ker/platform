using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;

namespace ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration
{

    [XmlRoot("Root")]
    public class XmlConfRoot
    {
        private XmlConfData _data;

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
        public XmlConfData Data
        {
            get => _data;

            set
            {
                _data = value;
                ((IChildItem<XmlConfRoot>)_data).Parent = this;
            }

        }

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
            //Начальная загрузка 
            XmlConfRoot conf = XmlConfHelper.DeserializeFromFile<XmlConfRoot>(path);
            
            //Инициализация компонентов данных
            conf.Data.LoadComponents();

            return conf;
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
    }
}
