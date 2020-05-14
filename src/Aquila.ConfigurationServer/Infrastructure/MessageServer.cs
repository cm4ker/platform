using System;
using NetMQ;
using NetMQ.Sockets;
using Aquila.IdeIntegration.Shared.Infrastructure;
using Aquila.IdeIntegration.Shared.Messages;

namespace Aquila.IdeIntegration.Server.Infrastructure
{
    public class MessageServer : IDisposable
    {
        private ResponseSocket _server;
        private NetMQPoller _poller;
        private MessageHandlerBackend _backend;

        public MessageServer()
        {
            _server = new ResponseSocket("@tcp://localhost:5556");
            _backend = new MessageHandlerBackend();
            _poller = new NetMQPoller();
            _server.ReceiveReady += ServerOnReceiveReady;
            _poller.Add(_server);
            _backend.SendMessage += BackendOnSendMessage;
        }

        public void Register(IMessageHandler handler)
        {
            _backend.Register(handler);
        }

        private void BackendOnSendMessage(object sender, MessageEventArgs e)
        {
            var frame = MessagePack.MessagePackSerializer.Typeless.Serialize(e.Message);
            _server.SendFrame(frame);
        }

        public void RunAsync()
        {
            _poller.RunAsync();
        }

        private void ServerOnReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            var frame = _server.ReceiveFrameBytes();
            var message = MessagePack.MessagePackSerializer.Typeless.Deserialize(frame) as PlatformMessage;
            if (message is null) throw new Exception("The message is unknown");
            _backend.Handle(message);
        }

        public void Dispose()
        {
            _poller?.Dispose();
            _server?.Dispose();
        }
    }
}