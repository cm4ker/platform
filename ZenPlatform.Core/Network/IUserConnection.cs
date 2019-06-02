using System.Net.Sockets;
using ZenPlatform.ServerClientShared.Network;
using ZenPlatform.ServerClientShared.Tools;

namespace ZenPlatform.Core.Network
{

    public interface IConnection<T>: IConnection
    { }

    public interface IUserMessageHandler : IMessageHandler { };
    public interface IAdminMessageHandler : IMessageHandler { };

    public interface IConnection: ISubscriber
    {
        /// <summary>
        /// Канал передачи данных
        /// </summary>
        IChannel Channel { get; }

        
        void Close();

        /// <summary>
        /// Открывает соединение с клиетом
        /// </summary>
        /// <param name="client">Клиен должен иметь статус Connected</param>
        void Open(TcpClient client);
    }
}