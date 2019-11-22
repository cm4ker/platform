using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Structure
{
    [XmlRoot("Root")]
    public class XCRoot : IXCRoot
    {
        private IXCData _data;
        private IXCRoles _roles;
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
            SessionSettings = new ChildItemCollection<IXCRoot, IXCSessionSetting>(this);

            //Берем счетчик по умолчанию
            _counter = new XCSimpleCounter();
        }

        public IXCConfigurationStorage Storage => _storage;
        public IXCConfigurationUniqueCounter Counter => _counter;

        /// <summary>
        /// Идентификатор конфигурации
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Имя конфигурации
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Версия конфигурации
        /// </summary>
        public string ProjectVersion { get; set; }

        /// <summary>
        /// Настройки сессии
        /// </summary>
        public ChildItemCollection<IXCRoot, IXCSessionSetting> SessionSettings { get; }


        /// <summary>
        /// Раздел данных
        /// </summary>
        public IXCData Data
        {
            get => _data;

            set
            {
                _data = value;
                ((IChildItem<XCRoot>) _data).Parent = this;
            }
        }

        /// <summary>
        /// Раздел  интерфейсов (UI)
        /// </summary>
        public IXCInterface Interface { get; set; }


        /// <summary>
        /// Раздел ролей
        /// </summary>
        public IXCRoles Roles
        {
            get => _roles;
            set
            {
                _roles = value;
                ((IChildItem<XCRoot>) _roles).Parent = this;
            }
        }

        /// <summary>
        /// Раздел модулей
        /// </summary>
        public IXCModules Modules { get; set; }

        
        /// <summary>
        /// Раздел переодических заданий
        /// </summary>
        public IXCSchedules Schedules { get; set; }

        
        /// <summary>
        /// Раздел языков
        /// </summary>
        public IXCLanguageList Languages { get; set; }

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
                var configurationTypes = new List<IXCType>();

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

        /// <summary>
        /// Сравнивает две конфигурации
        /// </summary>
        /// <param name="another"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object CompareConfiguration(IXCRoot another)
        {
            //TODO: Сделать механизм сравнения двух конфигураций
            throw new NotImplementedException();
        }
    }
}