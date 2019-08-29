using System;
using System.Collections.Generic;
using System.IO;

namespace ZenPlatform.Configuration
{
    /// <summary>
    /// Стратегия загрузки/сохранения данных конфигурации.
    /// Есть возмо
    /// </summary>
    public interface IXCConfigurationStorage : IXCConfigurationUniqueCounter
    {
        /// <summary>
        /// Получить двоичный объект
        /// </summary>
        /// <param name="name">Имя объекта</param>
        /// <param name="route">Параметр маршрутизации</param>
        /// <returns></returns>
        Stream GetBlob(string name, string route);

        /// <summary>
        /// Сохранить двоичный объект
        /// </summary>
        /// <param name="name">Имя объекта</param>
        /// <param name="route">Параметр маршрутизации</param>
        /// <param name="bytes">Данные</param>
        void SaveBlob(string name, string route, Stream stream);

        /// <summary>
        /// Получить корневой объект
        /// </summary>
        /// <returns></returns>
        Stream GetRootBlob();

        /// <summary>
        /// Сохранить корневой объект
        /// </summary>
        void SaveRootBlob(Stream stream);

    }


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
        IDictionary<Guid, string> GeteSettings();

        /// <summary>
        /// Созранить настройки объектов для их последующего восстановления
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
    public interface IXCConfigurationUniqueCounter
    {
        /// <summary>
        /// Получить идентификатор. Для файлового хранилища всё просто, он не привязан к данным, поэтому здесь будет просто
        /// Возвращаться инкремент, но для базы данных, необходимо вытаскивать из таблицы сопоставлений guid - тип объекта
        /// </summary>
        /// <param name="confId">Идентификатор конфигурации</param>
        /// <param name="oldId">Старый идентификатор, елси 0, в таком случае присваивается новый, елси не 0, тогда присваивается старый идентификатор</param>
        void GetId(Guid confId, ref uint oldId);
    }
}