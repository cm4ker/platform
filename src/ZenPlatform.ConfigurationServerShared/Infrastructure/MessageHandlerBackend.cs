using System;
using System.Collections.Generic;
using ZenPlatform.IdeIntegration.Shared.Messages;

namespace ZenPlatform.IdeIntegration.Shared.Infrastructure
{
    /// <summary>
    ///  Бэкент обработки сообщений с возможностью расширения кастомными обработчиками
    /// </summary>
    public class MessageHandlerBackend
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