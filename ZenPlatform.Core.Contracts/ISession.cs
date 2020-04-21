using System;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Contracts.Environment;
using ZenPlatform.Core.Tools;
using ZenPlatform.Data;

namespace ZenPlatform.Core.Contracts
{
    public interface ISession : IRemovable
    {
        /// <summary>
        /// Идентификатор сессии
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Пользователь
        /// </summary>
        IUser User { get; }


        /// <summary>
        /// Текущий контекст доступа к базе данных
        /// </summary>
        DataContext DataContext { get; }

        /// <summary>
        /// Установить параметр сессии
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetSessionParameter(string key, object value);


        /// <summary>
        /// Получить параметр сессии
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        object GetSessionParameter(string key, object value);


        /// <summary>
        /// Окружение
        /// </summary>
        IEnvironment Environment { get; }

        /// <summary>
        /// Закрыть
        /// </summary>
        void Close();
    }
}