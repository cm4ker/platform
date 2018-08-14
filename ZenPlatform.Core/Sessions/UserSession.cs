using System;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Environment;

namespace ZenPlatform.Core.Sessions
{
    /// <summary>
    /// Пользовательская сессия - по виду этой сессии мы можем запрещать незакконные операции - например мигрирование базы данных
    /// Пользователь не может выполнять инструкции связанные с изменением схемы, он лишь манипулирует данными
    /// </summary>
    public class UserSession : Session<WorkEnvironment>
    {
        private readonly User _user;

        public UserSession(WorkEnvironment env, User user, int id) : base(env, id)
        {
            _user = user;
        }

        public User User => _user;

        // Задача ниже V - не пойму для чего она. У сессии есть доступ к среде, не понятно, зачем для каждой сессии генерировать свой набор компонент.
        // Это скажется на потреблении памяти, это раз а два - это необходимость переинициализировать все компоненты в случае динамического обновления.
        // Легче держать все компоненты в одном месте, т.е. в среде и обращаться к ним из сессии. Я подумаю над этим...
        //TODO: Все компоненты инициализируются для сессии, так что необходимо, чтобы компоненты были доступны из сессии, либо на уровне ниже

        public void SetGlobalParameter(string key, object value)
        {
            Environment.Globals[key] = value;
        }

        public object GetGlobalParameter(string key, object value)
        {
            return Environment.Globals[key];
        }

        /// <summary>
        /// Получить метаданные
        /// </summary>
        /// <param name="type">Тип DTO</param>
        /// <returns></returns>
        public EntityMetadata GetMetadata(Type type)
        {
            return Environment.GetMetadata(type);
        }

        /// <summary>
        /// Получить метаданные
        /// </summary>
        /// <param name="typeId">Идентификатор типа</param>
        /// <returns></returns>
        public EntityMetadata GetMetadata(Guid typeId)
        {
            return Environment.GetMetadata(typeId);
        }

        /// <summary>
        /// Получить менеджер сущности
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEntityManager GetManager(Type type)
        {
            return Environment.GetManager(type);
        }
    }
}