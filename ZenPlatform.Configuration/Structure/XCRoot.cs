﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Structure
{
    [XmlRoot("Root")]
    public class XCRoot
    {
        private XCData _data;
        private XCRoles _roles;
        private IXCConfigurationStorage _storage;

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

        public IXCConfigurationStorage Storage => _storage;

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
                ((IChildItem<XCRoot>)_data).Parent = this;
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
                ((IChildItem<XCRoot>)_roles).Parent = this;
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
        /// <param name="storage"></param>
        /// <returns></returns>
        public static XCRoot Load(IXCConfigurationStorage storage)
        {
            XCRoot conf;
            using (var stream = storage.GetRootBlob())
                //Начальная загрузка 
                conf = XCHelper.DeserializeFromStream<XCRoot>(stream);

            //Сохраняем хранилище
            conf._storage = storage;

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
        public static XCRoot Create(string projectName)
        {
            if (string.IsNullOrEmpty(projectName))
                throw new InvalidOperationException();

            return new XCRoot()
            {
                ProjectId = Guid.NewGuid(),
                ProjectName = projectName
            };
        }

        /// <summary>
        /// Сохранить конфигурацию
        /// </summary>
        public void Save()
        {
            using (MemoryStream sw = new MemoryStream())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(XCRoot));
                serializer.Serialize(sw, this);

                //Сохранение раздела данных
                Data.Save();

                //Сохранение раздела ролей
                Roles.Save();

                //Сохранение раздела интерфейсов

                //Сохранение раздела ...
                _storage.SaveRootBlob(sw);
                //TODO: Необходимо инициировать сохранение для всех компонентов
            }
        }

        /// <summary>
        /// Созранить объект в контексте другого хранилища
        /// </summary>
        /// <param name="storage"></param>
        public void Save(IXCConfigurationStorage storage)
        {
            //Всё просто, подменяем хранилище, сохраняем, заменяем обратно

            var actualStorage = _storage;

            _storage = storage;

            Save();

            _storage = actualStorage;
        }


        //TODO: Сделать механизм сравнения двух конфигураций
    }
}