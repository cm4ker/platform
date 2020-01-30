using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Configuration.Contracts
{
    /// <summary>
    /// Интерфейс предоставляет доступ к дополнительным настройкам конфигурации
    /// Эти настройки могут быть переменны от одной базы к другой.
    ///
    /// Например. У нас есть имена колонок баз данных, которые необходимо хранить отдельно от конфигурации, потому что это генерируемая информация
    /// во время мигрирования.
    /// </summary>
    public interface IXCConfigurationSettingsStore
    {
        /// <summary>
        /// Получить следующий уникальный id
        /// </summary>
        /// <returns></returns>
        int GetNextUniqueId();

        /// <summary>
        /// Получить настройки для объектов
        /// </summary>
        /// <returns></returns>
        IDictionary<Guid, string> GetSettings();

        /// <summary>
        /// Сохранить настройки объектов для их последующего восстановления
        /// </summary>
        /// <param name="guid">Глобальный идентификатор объекта</param>
        /// <param name="id">Уникальный идентификатор объекта в разрезе базы  данных </param>
        /// <param name="content">Дополнительные свойства, в формате json</param>
        void SaveSetting(Guid guid, int id, string content);

        /// <summary>
        /// Удалить настройку базы данных
        /// </summary>
        /// <param name="id"></param>
        void DeleteSetting(Guid id);
    }

    /// <summary>
    /// Модель настроек объекта
    /// </summary>
    public class ObjectSettings
    {
        /// <summary>
        /// Глобальный уникальный идентификатор объекта
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// Дополнительные свойства 
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Уникальный номер в базе данных
        /// </summary>
        public int Id { get; set; }
    }


    /// <summary>
    /// Интерфейс уникального счётчика
    /// </summary>
    public interface IUniqueCounter
    {
        /// <summary>
        /// Получить идентификатор. Для файлового хранилища всё просто, он не привязан к данным, поэтому здесь будет просто
        /// Возвращаться инкремент, но для базы данных, необходимо вытаскивать из таблицы сопоставлений guid - тип объекта
        /// </summary>
        /// <param name="confId">Идентификатор конфигурации</param>
        /// <param name="oldId">Старый идентификатор, елси 0, в таком случае присваивается новый, елси не 0, тогда присваивается старый идентификатор</param>
        uint GetId(Guid confId);
    }


    public interface ISettingsManager
    {
        IEnumerable<IObjectSetting> GetSettings();

        void AddOrUpdateSetting(IObjectSetting setting);
    }
}