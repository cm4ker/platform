using System.Collections.Generic;
using ZenPlatform.Core.Sessions;
using ZenPlatform.Core.Network;
using ZenPlatform.Core.Authentication;
using ZenPlatform.QueryBuilder;
using System;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Data;

namespace ZenPlatform.Core.Environment
{
    /// <summary>
    ///  Среда для подключения. Обеспечивает некий контекст в котором работает удаленный пользователь
    /// </summary>
    public interface IEnvironment<in TConfiguration>
        where TConfiguration : class
    {
        /// <summary>
        /// Имя среды
        /// </summary>
        string Name { get; }

        /// <summary>
        ///  Текущие сессии
        /// </summary>
        IList<ISession> Sessions { get; }


        /// <summary>
        /// Сервис RPC
        /// </summary>
        IInvokeService InvokeService { get; }


        /// <summary>
        /// Инициализация
        /// </summary>
        /// <param name="config">Конфигурация</param>
        void Initialize(TConfiguration config);

        /// <summary>
        /// Менеджер аутентификации
        /// </summary>
        IAuthenticationManager AuthenticationManager { get; }

        /// <summary>
        /// Создать сессию
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <returns>Экземпляр сессии</returns>
        ISession CreateSession(IUser user);
    }
}