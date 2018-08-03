using System;
using System.Collections.Generic;

namespace ZenPlatform.Configuration
{
    /// <summary>
    /// Стратегия загрузки/сохранения данных конфигурации.
    /// Есть возмо
    /// </summary>
    public interface IXCConfigurationStorage
    {
        /// <summary>
        /// Получить двоичный объект
        /// </summary>
        /// <param name="name">Имя объекта</param>
        /// <param name="route">Параметр маршрутизации</param>
        /// <returns></returns>
        byte[] GetBlob(string name, string route);

        /// <summary>
        /// Получить строковый объект
        /// </summary>
        /// <param name="name">Имя объекта</param>
        /// <param name="route">Параметр маршрутизации</param>
        /// <returns></returns>
        string GetStringBlob(string name, string route);

        /// <summary>
        /// Сохранить двоичный объект
        /// </summary>
        /// <param name="name">Имя объекта</param>
        /// <param name="route">Параметр маршрутизации</param>
        /// <param name="bytes">Данные</param>
        void SaveBlob(string name, string route, byte[] bytes);

        /// <summary>
        /// Сохранить строковый объект
        /// </summary>
        /// <param name="name">Имя объекта</param>
        /// <param name="route">Параметр маршрутизации</param>
        /// <param name="data">Строковые данные</param>
        void SaveBlob(string name, string route, string data);

        /// <summary>
        /// Получить корневой объект
        /// </summary>
        /// <returns></returns>
        byte[] GetRootBlob();

        /// <summary>
        /// Получить корневой объект в виде строки
        /// </summary>
        /// <returns></returns>
        string GetStringRootBlob();

        /// <summary>
        /// Сохранить корневой объект
        /// </summary>
        void SaveRootBlob(string content);
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
}