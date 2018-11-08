﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Structure
{
    [XmlRoot("Root")]
    public class XCRoot
    {
        private XCData _data;
        private XCRoles _roles;
        private IXCConfigurationStorage _storage;
        private IXCConfigurationUniqueCounter _counter;

        public XCRoot()
        {
            ProjectId = Guid.NewGuid();

            Data = new XCData();
            Interface = new XCInterface();
            Roles = new XCRoles();
            Modules = new XCModules();
            Schedules = new XCSchedules();
            Languages = new XCLanguageList();
            SessionSettings = new ChildItemCollection<XCRoot, XCSessionSetting>(this);

            //Берем счетчик по умолчанию
            _counter = new XCSimpleCounter();
        }

        public IXCConfigurationStorage Storage => _storage;
        public IXCConfigurationUniqueCounter Counter => _counter;
        
        /// <summary>
        /// Идентификатор конфигурации
        /// </summary>
        [XmlElement("ProjectId")]
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Имя конфигурации
        /// </summary>
        [XmlElement("ProjectName")]
        public string ProjectName { get; set; }

        /// <summary>
        /// Версия конфигурации
        /// </summary>
        [XmlElement("ProjectVersion")]
        public string ProjectVersion { get; set; }

        /// <summary>
        /// Настройки сессии
        /// </summary>
        [XmlArray("SessionSettings")]
        [XmlArrayItem(ElementName = "SessionSetting", Type = typeof(XCSessionSetting))]
        public ChildItemCollection<XCRoot, XCSessionSetting> SessionSettings { get; }

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
        public XCLanguageList Languages { get; set; }

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
            conf._counter = storage;
            
            //Инициализация компонентов данных
            conf.Data.Load();

            //Инициализация ролевой системы
            conf.Roles.Load();

            //Инициализация параметров сессии
            conf.LoadSessionSettings();

            return conf;
        }

        private void LoadSessionSettings()
        {
            foreach (var setting in SessionSettings)
            {
                var configurationTypes = new List<XCTypeBase>();

                foreach (var propertyType in setting.Types)
                {
                    var type = Data.PlatformTypes.FirstOrDefault(x => x.Guid == propertyType.Guid);
                    //Если по какой то причине тип поля не найден, в таком случае считаем, что конфигурация битая и выкидываем исключение
                    if (type == null) throw new Exception("Invalid configuration");

                    configurationTypes.Add(type);
                }

                //После того, как мы получили все типы мы обязаны очистить битые ссылки и заменить их на нормальные 
                setting.Types.Clear();
                setting.Types.AddRange(configurationTypes);
            }
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
           

            //Сохранение раздела данных
            Data.Save();

            //Сохранение раздела ролей
            Roles.Save();

            //Сохранение раздела интерфейсов

            //Сохранение раздела ...
            
            var ms = this.SerializeToStream();
            _storage.SaveRootBlob(ms);
            //TODO: Необходимо инициировать сохранение для всех компонентов
        }

        /// <summary>
        /// Созранить объект в контексте другого хранилища
        /// </summary>
        /// <param name="storage"></param>
        public void Save(IXCConfigurationStorage storage)
        {
            //Всё просто, подменяем хранилище, сохраняем, заменяем обратно
            var actualStorage = _storage;
            var actualCounter = _counter;
            _storage = storage;
            _counter = storage;
            Save();
            _storage = actualStorage;
            _counter = actualCounter;
        }


        //TODO: Сделать механизм сравнения двух конфигураций
    }


    public class XCLanguageList : List<XCLanguage>
    {
    }
}