using System;
using System.IO;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Structure
{
    public interface IConfigurationManipulator
    {
        IXCRoot Load(IXCConfigurationStorage storage);

        IXCRoot Create(string projectName);
        
        Stream SaveToStream(IXCRoot root);

        string GetHash(IXCRoot root);
    }


    public interface IXCRoot
    {
        IXCConfigurationStorage Storage { get; }
        IXCConfigurationUniqueCounter Counter { get; }

        /// <summary>
        /// Идентификатор конфигурации
        /// </summary>
        Guid ProjectId { get; set; }

        /// <summary>
        /// Имя конфигурации
        /// </summary>
        string ProjectName { get; set; }

        /// <summary>
        /// Версия конфигурации
        /// </summary>
        string ProjectVersion { get; set; }

        /// <summary>
        /// Настройки сессии
        /// </summary>
        ChildItemCollection<IXCRoot, IXCSessionSetting> SessionSettings { get; }

        /// <summary>
        /// Раздел данных
        /// </summary>
        IXCData Data { get; set; }

        /// <summary>
        /// Раздел  интерфейсов (UI)
        /// </summary>
        IXCInterface Interface { get; set; }

        /// <summary>
        /// Раздел ролей
        /// </summary>
        IXCRoles Roles { get; set; }

        /// <summary>
        /// Раздел модулей
        /// </summary>
        IXCModules Modules { get; set; }

        /// <summary>
        /// Раздел переодических заданий
        /// </summary>
        IXCSchedules Schedules { get; set; }

        /// <summary>
        /// Раздел языков
        /// </summary>
        IXCLanguageList Languages { get; set; }

        /// <summary>
        /// Сохранить конфигурацию
        /// </summary>
        void Save();

        /// <summary>
        /// Созранить объект в контексте другого хранилища
        /// </summary>
        /// <param name="storage"></param>
        void Save(IXCConfigurationStorage storage);

        /// <summary>
        /// Сравнивает две конфигурации
        /// </summary>
        /// <param name="another"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        object CompareConfiguration(IXCRoot another);
    }
}