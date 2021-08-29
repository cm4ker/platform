using System.Collections.Generic;
using Aquila.Core.Contracts.Authentication;
using Aquila.Core.Contracts.Network;

namespace Aquila.Core.Contracts.Instance
{
    public interface IInstance
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
}