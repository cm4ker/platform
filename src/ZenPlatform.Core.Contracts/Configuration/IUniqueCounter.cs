using System;

namespace ZenPlatform.Configuration.Contracts
{
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
}