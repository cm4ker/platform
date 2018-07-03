using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace ZenPlatform.Configuration.ConfigurationLoader.Structure
{
    [XmlRoot("Root")]
    public class XCRoot
    {
        private XCData _data;
        private XCRoles _roles;

        public XCRoot()
        {
            ProjectId = Guid.NewGuid();

            Data = new XCData();
            Interface = new XCInterface();
            Roles = new XCRoles();
            Modules = new XCModules();
            Schedules = new XCSchedules();
            Languages = new List<XCLanguage>();
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

        [XmlElement] public XCInterface Interface { get; set; }

        [XmlElement]
        public XCRoles Roles
        {
            get => _roles;
            set
            {
                _roles = value;
                ((IChildItem<XCRoot>) _roles).Parent = this;
            }
        }

        [XmlElement] public XCModules Modules { get; set; }

        [XmlElement] public XCSchedules Schedules { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "Language", Type = typeof(XCLanguage))]
        public List<XCLanguage> Languages { get; set; }

        /// <summary>
        /// Загрузить концигурацию
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static XCRoot Load(string path)
        {
            //Начальная загрузка 
            XCRoot conf = XCHelper.DeserializeFromFile<XCRoot>(path);

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