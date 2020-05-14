using System;
using System.IO;
using System.Net.Sockets;
using Aquila.Core.Tools;

namespace Aquila.Core.Network
{
    public interface IConnection
    {
        IChannel Channel { get; }
        /// <summary>
        /// Закрывает соединение
        /// </summary>
        void Close();

        /// <summary>
        /// Информация о подключении
        /// </summary>
        ConnectionInfo Info { get; }


        void Open();

        /// <summary>
        /// Возвращает True если соединение открыто
        /// </summary>
        bool Opened { get; }

        IDisposable Subscribe(IConnectionObserver<IConnectionContext> observer);
    }
}