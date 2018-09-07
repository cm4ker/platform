using System;
using MessagePack;

namespace ZenPlatform.ConfigurationServerMessages.Messages
{
    /// <summary>
    /// Запрос на дочерние элементы элемента конфигурации
    /// </summary>
    [MessagePackObject]
    public class XCTreeRequestMessage : XCMessage
    {
        /// <summary>
        /// Идентификатор элемента, для которого запрашиваем поддерево
        /// </summary>
        [Key(0)]
        public Guid ItemId { get; set; }

        /// <summary>
        /// Идентификатор запроса, необходим для ассинхронной модели, чтобы знать на что мы отвечаем
        /// </summary>
        [Key(1)]
        public Guid RequestId { get; set; }
    }
}
