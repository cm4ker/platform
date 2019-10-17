using System.IO;
using System.Net;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Contracts;

namespace ZenPlatform.Core.Network.Contracts
{
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

    /// <summary>
    /// Описывает
    /// </summary>
    public interface IProtocolClient
    {
        /// <summary>
        /// Показывает, подключен ли сейчас клиент
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Информация о соединении
        /// </summary>
        ConnectionInfo Info { get; }

        /// <summary>
        /// Закрыть соединение
        /// </summary>
        void Close();

        /// <summary>
        /// Соединиться
        /// </summary>
        /// <param name="endPoint"></param>
        void Connect(IPEndPoint endPoint);

        /// <summary>
        /// Выполнить
        /// </summary>
        /// <param name="route">Маршрут</param>
        /// <param name="args">Аргументы</param>
        /// <typeparam name="TResponce">Тип возвращаемого ответа</typeparam>
        /// <returns>Возвращает объект - результат выполнения удалённой процедуры</returns>
        TResponce Invoke<TResponce>(Route route, params object[] args);


        /// <summary>
        /// Вызвать и получить результат в виде потока
        /// </summary>
        /// <param name="route"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        Stream InvokeAsStream(Route route, params object[] args);

        /// <summary>
        /// Получить сервис
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetService<T>();
    }
}