using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.ConfigurationServerMessages.Models;
using ZenPlatform.IdeIntegration.Messages.Messages;

namespace ZenPlatform.IdeIntegration.Server.Infrastructure
{
    /// <summary>
    ///  Сервер, обслуживающий непосредственно сообщения
    /// </summary>
    public sealed class MessageHandlerBackend
    {
        private List<IMessageHandler> _handlers;

        public MessageHandlerBackend()
        {
            _handlers = new List<IMessageHandler>();
        }

        /// <summary>
        /// Обработать сообщение. Сообщение будет передано во все зарегистрированные обработчики
        /// </summary>
        /// <param name="message"></param>
        public void Handle(PlatformMessage message)
        {
            var args = new MessageEventArgs(message);

            _handlers.ForEach(x => x.TryHandle(args));
        }

        /// <summary>
        /// Зарегистрировать обработчик
        /// </summary>
        /// <param name="handler"></param>
        public void Register(IMessageHandler handler)
        {
            _handlers.Add(handler);
            handler.SendMessage += HandlerOnSendMessage;
        }

        private void HandlerOnSendMessage(object sender, MessageEventArgs e)
        {
            OnSendMessage(sender as IMessageHandler, e);
        }

        public event EventHandler<MessageEventArgs> SendMessage;

        /// <summary>
        /// При отправке сообщения.
        /// Вызывается, когда сервер отправляет сообщение клиенту
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="args"></param>
        protected void OnSendMessage(IMessageHandler handler, MessageEventArgs args)
        {
            SendMessage?.Invoke(handler, args);
        }
    }

    /// <summary>
    /// Аргументы обработки сообщений
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(PlatformMessage message)
        {
            Message = message;
        }

        /// <summary>
        /// Обрабатываемое сообщение
        /// </summary>
        public PlatformMessage Message { get; }
    }

    public class MessageServer : IDisposable
    {
        private ResponseSocket _server;
        private NetMQPoller _poller;
        private MessageHandlerBackend _backend;

        public MessageServer()
        {
            _server = new ResponseSocket("@tcp://localhost:5556");
            _backend = new MessageHandlerBackend();
            _backend.Register(new ConfigurationMessageHandler());
            _poller = new NetMQPoller();
            _server.ReceiveReady += ServerOnReceiveReady;
            _poller.Add(_server);

            _backend.SendMessage += BackendOnSendMessage;
        }

        private void BackendOnSendMessage(object sender, EventArgs e)
        {
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

    /// <summary>
    /// Обработчик сообщений
    /// </summary>
    public interface IMessageHandler
    {
        bool TryHandle(MessageEventArgs args);
        event EventHandler<MessageEventArgs> SendMessage;
    }

    public class ConfigurationMessageHandler : IMessageHandler
    {
        private readonly XCRoot _conf;

        public ConfigurationMessageHandler(XCRoot conf)
        {
            _conf = conf;
        }

        public bool TryHandle(MessageEventArgs args)
        {
            if (!(args.Message is XCHelloMessage helloMessage)) return false;

            Debug.WriteLine("We receive hello message and handle it");
            helloMessage.Handled = true;
            return true;
        }

        private void Handle(XCTreeRequestMessage treeRequest)
        {
            var responce = new XCTreeResponceMessage();

            switch (treeRequest.ItemType)
            {
                case XCTreeItemType.Root:
                {
                    responce.RequestId = treeRequest.RequestId;
                    responce.ParentId = treeRequest.ItemId;

                    responce.Items.Add(new XCItem() {ItemId = });
                    break;
                }
                case XCTreeItemType.Data:
                {
                    break;
                }
            }
        }

        public event EventHandler<MessageEventArgs> SendMessage;

        protected virtual void OnSendMessage(MessageEventArgs e)
        {
            SendMessage?.Invoke(this, e);
        }
    }
}