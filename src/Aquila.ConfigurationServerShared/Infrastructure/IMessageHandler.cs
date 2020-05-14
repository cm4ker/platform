using System;

namespace Aquila.IdeIntegration.Shared.Infrastructure
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