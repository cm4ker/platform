using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Contracts.Network;

namespace ZenPlatform.Core.Network.Contracts
{
    /// <summary>
    /// Уровень платформы
    /// </summary>
    public interface IPlatformClient : IProtocolClient
    {
        /// <summary>
        /// Аутентификация пройдена
        /// </summary>
        bool IsAuthenticated { get; }

        bool Authenticate(IAuthenticationToken token);

        /// <summary>
        /// Текущая база данных
        /// </summary>
        string Database { get; }

        bool IsUse { get; }

        /// <summary>
        /// Использовать базу данных
        /// </summary>
        /// <param name="name">Имя базы данных</param>
        /// <returns></returns>
        bool Use(string name);
    }
}