using System.IO;
using System.Net;

namespace Aquila.Core.Contracts.Network
{
    /// <summary>
    /// Уровень протокола
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
        IConnectionInfo Info { get; }

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
        /// <typeparam name="TResponse">Тип возвращаемого ответа</typeparam>
        /// <returns>Возвращает объект - результат выполнения удалённой процедуры</returns>
        TResponse Invoke<TResponse>(Route route, params object[] args);


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