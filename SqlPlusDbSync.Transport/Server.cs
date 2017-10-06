using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SqlPlusDbSync.Transport
{
    public class Server
    {
        private readonly int _port;
        TcpListener _listener;

        public Server(int port)
        {
            _port = port;
            int MaxThreadsCount = Environment.ProcessorCount * 4;

            ThreadPool.SetMaxThreads(MaxThreadsCount, MaxThreadsCount);
            ThreadPool.SetMinThreads(2, 2);
        }


        public void Start()
        {
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();


            while (true)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(ClientThread), _listener.AcceptTcpClient());
            }
        }

        static void ClientThread(Object StateInfo)
        {
            var client = new Client((TcpClient)StateInfo);
            var cp = new CommandProcesor(client);
            cp.StartProcess();
        }

        ~Server()
        {
            if (_listener != null)
            {
                _listener.Stop();
            }
        }

    }
}