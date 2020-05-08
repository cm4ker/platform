using System;
using NetMQ;
using NetMQ.Sockets;
using Aquila.Configuration;
using Aquila.IdeIntegration.Shared.Infrastructure;
using Aquila.IdeIntegration.Shared.Messages;
using Aquila.IdeIntegration.Shared.Models;

namespace Aquila.IdeIntegration.Client.Infrastructure
{
    public class MessageClient : IDisposable
    {
        private RequestSocket _client;
        private string _connString;
        private ClientMessageBackend _backend;

        public MessageClient(string host, int port)
        {
            _connString = $">tcp://{host}:{port}";
            _client = new RequestSocket();
            _client.ReceiveReady += ClientOnReceiveReady;
            _backend = new ClientMessageBackend();
        }

        public void Connect()
        {
            _client.Connect(_connString);
        }

        /// <summary>
        /// Получить дерево элементов по элементу
        /// </summary>
        /// <param name="item"></param>
        public void RequestItems(XCItem item)
        {
            RequestItems(item.ItemId, item.ItemType);
        }

        /// <summary>
        /// Получить дерево элементов 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="nodeKind"></param>
        public void RequestItems(Guid itemId, XCNodeKind nodeKind)
        {
            var message = new XCTreeRequestMessage(itemId, Guid.NewGuid(), nodeKind);
            Send(message);
        }

        /// <summary>
        /// Обработать сообщение
        /// </summary>
        /// <param name="args">Аргумент</param>
        /// <returns></returns>
        private bool Handle(MessageEventArgs args)
        {
            if (args.Message is XCTreeResponceMessage)
            {
                OnConfTreeFetch(new MessageEventArgs(args.Message));
            }

            return true;
        }

        public void Send(PlatformMessage message)
        {
            var bytes = MessagePack.MessagePackSerializer.Typeless.Serialize(message);
            _client.SendFrame(bytes);
        }

        private void ClientOnReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            var frame = _client.ReceiveFrameBytes();
            var message = MessagePack.MessagePackSerializer.Typeless.Deserialize(frame) as PlatformMessage;
            if (message is null) throw new Exception("The message is unknown");
            Handle(new MessageEventArgs(message));
        }


        #region Events

        /// <summary>
        /// Событие происходит когда мы получили кусок дерева от сервера
        /// </summary>
        public event EventHandler<MessageEventArgs> ConfTreeFetch;

        protected virtual void OnConfTreeFetch(MessageEventArgs e)
        {
            ConfTreeFetch?.Invoke(this, e);
        }

        #endregion

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}