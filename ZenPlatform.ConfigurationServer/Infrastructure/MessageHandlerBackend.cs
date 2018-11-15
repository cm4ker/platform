using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
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
}