using MessagePack;

namespace ZenPlatform.IdeIntegration.Messages.Messages
{
    /// <summary>
    /// Базовый тип сообщения
    /// </summary>
    [MessagePackObject]
    public abstract class PlatformMessage
    {
        /// <summary>
        /// Компонент, которому предназначается данное сообщение
        /// </summary>
        [Key(100000)]
        public string ComponentName { get; set; }
    }


    /// <summary>
    /// Сообщение приветствия
    /// </summary>
    [MessagePackObject]
    public class XCHelloMessage : PlatformMessage
    {
        [Key(0)]
        public string TextMessage { get; set; } = "Hello world";

        [Key(1)]
        public bool Handled { get; set; }
    }
}