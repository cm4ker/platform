using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Renci.SshNet;

namespace ZenPlatform.Core.Network
{
    public interface IConnectionContext
    {
        IConnection Connection { get; }
    }


    public interface ITransportClientFactory
    {
        ITransportClient Create(IPEndPoint endPoint);
    }

    public class TCPTransportClientFactory : ITransportClientFactory
    {
        public ITransportClient Create(IPEndPoint endPoint)
        {

            var c = new TcpClient();
            c.Connect(endPoint);
            
            return new TCPTransportClient(c);
        }
    }

    public class SSHTransportClientFactory : ITransportClientFactory
    {
        public ITransportClient Create(IPEndPoint endPoint)
        {
            SshClient sshClient = new SshClient(endPoint.Address.ToString(), endPoint.Port, "hello", "hello");
            sshClient.Connect();

            var client = new SSHTransportClient(sshClient);

            return client;
        }
    }
}