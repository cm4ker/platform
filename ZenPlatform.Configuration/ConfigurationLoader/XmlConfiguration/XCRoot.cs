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
    public class XCRoot
    {
        private XCData _data;
        private XmlConfRoles _roles;

        public XCRoot()
        {
            ProjectId = Guid.NewGuid();

            Data = new XCData();
            Interface = new XmlConfInterface();
            Roles = new XmlConfRoles();
            Modules = new XmlConfModules();
            Schedules = new XmlConfSchedules();
            Languages = new List<XmlConfLanguage>();
        }

        [XmlElement("ProjectId")] public Guid ProjectId { get; set; }

        [XmlElement("ProjectName")] public string ProjectName { get; set; }

        [XmlElement("ProjectVersion")] public string ProjectVersion { get; set; }

        [XmlElement(Type = typeof(XCData), ElementName = "Data")]
        public XCData Data
        {
            get => _data;

            set
            {
                _data = value;
                ((IChildItem<XCRoot>) _data).Parent = this;
            }
        }

        [XmlElement] public XmlConfInterface Interface { get; set; }

        [XmlElement]
        public XmlConfRoles Roles
        {
            get => _roles;
            set
            {
                _roles = value;
                ((IChildItem<XCRoot>) _roles).Parent = this;
            }
        }

        [XmlElement] public XmlConfModules Modules { get; set; }

        [XmlElement] public XmlConfSchedules Schedules { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "Language", Type = typeof(XmlConfLanguage))]
        public List<XmlConfLanguage> Languages { get; set; }

        /// <summary>
        /// Загрузить концигурацию
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static XCRoot Load(string path)
        {
            //Начальная загрузка 
            XCRoot conf = XmlConfHelper.DeserializeFromFile<XCRoot>(path);

            //Инициализация компонентов данных
            conf.Data.Load();

            //Инициализация ролевой системы
            conf.Roles.Load();


            return conf;
        }

        /// <summary>
        /// Создать новую концигурацию
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static XCRoot Create(string path)
        {
            return new XCRoot()
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
                XmlSerializer serializer = new XmlSerializer(typeof(XCRoot));
                serializer.Serialize(tr, this);
            }
        }
    }
}