using System;
using System.Security.Cryptography;
using NetMQ;
using NetMQ.Sockets;
using ZenPlatform.IdeIntegration.Shared.Messages;

namespace ZenPlatform.IdeIntegration.ReactiveClient
{
    public class MessageClient : IDisposable
    {
        private RequestSocket _client;
        private string _connString;

        public MessageClient(string host, int port)
        {
            _connString = $">tcp://{host}:{port}";
            _client = new RequestSocket();
            _client.ReceiveReady += ClientOnReceiveReady;
        }

        public void Connect()
        {
            _client.Connect(_connString);
        }

        private void ClientOnReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            var frame = _client.ReceiveFrameBytes();
            var message = MessagePack.MessagePackSerializer.Typeless.Deserialize(frame) as PlatformMessage;
            if (message is null) throw new Exception("The message is unknown");
            _backend.Handle(message);
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}