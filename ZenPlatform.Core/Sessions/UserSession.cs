using System;
using System.Collections.Concurrent;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.CacheService;
using ZenPlatform.Core.Environment;
using ZenPlatform.Data;

namespace ZenPlatform.Core.Sessions
{
    /// <summary>
    /// Пользовательская сессия - по виду этой сессии мы можем запрещать незакконные операции - например мигрирование базы данных
    /// Пользователь не может выполнять инструкции связанные с изменением схемы, он лишь манипулирует данными
    /// </summary>
    public class UserSession : Session
    {
        private readonly ConcurrentDictionary<string, object> _sessionParameters;

        public UserSession(WorkEnvironment env, IUser user, IDataContextManager dataContextManger,
            ICacheService cacheService)
            : base(env, dataContextManger, cacheService)
        {
            _sessionParameters = new ConcurrentDictionary<string, object>();
            User = user;
        }

        public override IUser User { get; protected set; }

        // Задача ниже V - не пойму для чего она. У сессии есть доступ к среде, не понятно, зачем для каждой сессии генерировать свой набор компонент.
        // Это скажется на потреблении памяти, это раз а два - это необходимость переинициализировать все компоненты в случае динамического обновления.
        // Легче держать все компоненты в одном месте, т.е. в среде и обращаться к ним из сессии. Я подумаю над этим...
        //TODO: Все компоненты инициализируются для сессии, так что необходимо, чтобы компоненты были доступны из сессии, либо на уровне ниже


        /// <summary>
        /// Установить параметр сессии
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public override void SetSessionParameter(string key, object value)
        {
            _sessionParameters[key] = value;
        }


        /// <summary>
        /// Получить параметр сесси
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object GetSessionParameter(string key, object value)
        {
            if (_sessionParameters.TryGetValue(key, out var result))
                return result;
            else
                return null;
        }

        /// <summary>
        /// Получить метаданные
        /// </summary>
        /// <param name="type">Тип DTO</param>
        /// <returns></returns>
        public EntityMetadata GetMetadata(Type type)
        {
            //    return Environment.GetMetadata(type);
            return null;
        }

        /// <summary>
        /// Получить метаданные
        /// </summary>
        /// <param name="typeId">Идентификатор типа</param>
        /// <returns></returns>
        public EntityMetadata GetMetadata(Guid typeId)
        {
            //    return Environment.GetMetadata(typeId);
            return null;
        }

        /// <summary>
        /// Получить менеджер сущности
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEntityManager GetManager(Type type)
        {
            //return Environment.GetManager(type);
            return null;
        }
    }
}