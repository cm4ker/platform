using System;

namespace ZenPlatform.IdeIntegration.Server.Infrastructure
{
    /// <summary>
    /// Обработчик сообщений
    /// </summary>
    public interface IMessageHandler
    {
        bool TryHandle(MessageEventArgs args);
        event EventHandler<MessageEventArgs> SendMessage;
    }
}