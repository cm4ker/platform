using System;
using ZenPlatform.Core;
using ZenPlatform.QueryBuilder;
using ZenPlatform.WorkProcess;

namespace ZenPlatform.ConnectionServer
{
    class Program
    {
        public static void Main()
        {
//            var wp = new SystemProcess(new StartupConfig()
//            {
//                ConnectionString = "host=localhost; database=db1; user id=postgres;  password=123456;",
//                DatabaseType = SqlDatabaseType.Postgres
//            });

            Console.WriteLine("Starting migration...");

            //wp.Migrate();

            Console.WriteLine("Migration done!");
        }
    }
}
//    {
//using System;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using Ether.Network;
//using Ether.Network.Common;
//using Ether.Network.Packets;
//using Ether.Network.Server;
//
//
//namespace ZenPlatform.ConnectionServer
//{
//    class Program
//    {
//
//        /*
//         * Connection server - сервер подключений. Прослойка, через которую пользователи получают доступ к приложению.
//         * Сам Application должен работать в отдельном приложении. Коммуникация между двумя этими компонентами должна осуществляться через какой-то сокетный интерфейс
//         * ConnectionServer выполяет роль посредника между ApplicationServer и ThinClient.
//         * 
//         * 1) Принять команду от сервера, отправить её клиенту
//         * 2) Принять команду от клиента и отправить её на сервер
//         * 3) Получить состояние сервера
//         * 4) .....
//         * 
//         * Необходимо определить набор команд которые доступны на ConnectionServer
//         * 
//         * 1) Подключение - обычная команда, которая примает connection пользователя.
//         * 2) Отправить 
//         */
//
//        static void Main(string[] args)
//        {
//            using (var server = new MyServer())
//                server.Start();
//        }
//        internal class MyServer : NetServer<ClientConnection>
//        {
//            public MyServer()
//            {
//                // Configure the server
//                this.Configuration.Backlog = 100;
//                this.Configuration.Port = 8888;
//                this.Configuration.MaximumNumberOfConnections = 100;
//                this.Configuration.Host = "127.0.0.1";
//            }
//
//            protected override void Initialize()
//            {
//                Console.WriteLine("Server is ready.");
//            }
//
//            protected override void OnClientConnected(ClientConnection connection)
//            {
//                Console.WriteLine("New client connected!");
//
//                connection.SendFirstPacket();
//            }
//
//            protected override void OnClientDisconnected(ClientConnection connection)
//            {
//                Console.WriteLine("Client disconnected!");
//            }
//
//            protected override void OnError(Exception exception)
//            {
//                throw new NotImplementedException();
//            }
//        }
//
//        internal class ClientConnection : NetConnection
//        {
//            public void SendFirstPacket()
//            {
//                using (var packet = new NetPacket())
//                {
//                    packet.Write("Welcome " + this.Id.ToString());
//
//                    this.Send(packet);
//                }
//            }
//
//            public override void HandleMessage(NetPacketBase packet)
//            {
//                string value = packet.Read<string>();
//
//                Console.WriteLine("Received '{1}' from {0}", this.Id, value);
//
//                using (var p = new NetPacket())
//                {
//                    p.Write(string.Format("OK: '{0}'", value));
//                    this.Send(p);
//                }
//            }
//        }
//    }
//}