using System;
using ZenPlatform.IdeIntegration.Shared.Messages;

namespace ZenPlatform.IdeIntegration.Shared.Infrastructure
{
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

        /// <summary>
        /// Обработано другим хендлером. Это сообщение уже не стоит обрабатывать. Но вы можете проигнорировать это свойство
        /// </summary>
        public bool Handled { get; set; }
    }
}