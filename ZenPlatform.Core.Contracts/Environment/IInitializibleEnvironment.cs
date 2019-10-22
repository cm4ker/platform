using System.Collections.Generic;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Network;
using ZenPlatform.Core.Sessions;

namespace ZenPlatform.Core.Environment.Contracts
{
    public interface IEnvironment
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

    /// <summary>
    ///  Среда для подключения. Обеспечивает некий контекст в котором работает удаленный пользователь
    /// </summary>
    public interface IInitializibleEnvironment<in TConfiguration> : IEnvironment
        where TConfiguration : class
    {
        /// <summary>
        /// Инициализация
        /// </summary>
        /// <param name="config">Конфигурация</param>
        void Initialize(TConfiguration config);
    }
}