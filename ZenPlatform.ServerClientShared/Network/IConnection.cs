using System.IO;
using System.Net.Sockets;
using ZenPlatform.ServerClientShared.Tools;

namespace ZenPlatform.ServerClientShared.Network
{
    public interface IConnection: IRemovable
    {

        /// <summary>
        /// Закрывает соединение
        /// </summary>
        void Close();

        /// <summary>
        /// Возвращает поток для обмена данными
        /// </summary>
        /// <returns>Возвращает поток связанный с соединением для обмена данными</returns>
        Stream GetStream();

        /// <summary>
        /// Информация о подключении
        /// </summary>
        ConnectionInfo Info { get; }

        /// <summary>
        /// Открывает соединение с клиетом
        /// </summary>
        /// <param name="client">Клиен должен иметь статус Connected</param>
        void Open(TcpClient client);

        /// <summary>
        /// Возвращает True если соединение открыто
        /// </summary>
        bool Opened { get; }
    }
}